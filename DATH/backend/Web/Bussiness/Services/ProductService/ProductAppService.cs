using AutoMapper;
using Bussiness.Dto;
using Bussiness.Interface.Core;
using Bussiness.Interface.ProductInterface;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Dapper;
using Database;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Bussiness.Services.ProductService
{
    public class ProductAppService : BaseService, IProductAppService
    {
        private readonly IRepository<Product, long> _productRepo;
        private readonly IPhotoService _photoService;
        private readonly IRepository<Specification, long> _specificationRepo;
        private readonly DataContext _dataContext;
        private readonly IDapper _dapper;
        private readonly IRepository<SpecificationCategory> _specificationCategoryRepo;

        public ProductAppService(
            IRepository<Product, long> productRepo, 
            IPhotoService photoService, 
            IRepository<Specification, long> specificationRepo, 
            DataContext dataContext, 
            IDapper dapper, 
            IRepository<SpecificationCategory> specificationCategoryRepo,
            IMapper mapper
            )
        {
            _productRepo = productRepo;
            _photoService = photoService;
            _specificationRepo = specificationRepo;
            _dataContext = dataContext;
            _dapper = dapper;
            _specificationCategoryRepo = specificationCategoryRepo;
            ObjectMapper = mapper;
        }

        #region AddPhoto
        public async Task<object?> AddPhoto(long id, bool isMain, long? specificationId, IFormFile file)
        {
            Product? product = await _productRepo.GetAsync(id);
            if (product == null) return null;

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return result.Error.Message;

            Photo? photo = new()
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                IsMain = isMain,
                SpecificationId = specificationId
            };

            product.Photos = new List<Photo> { photo };

            ProductForViewDto? res = new();
            ObjectMapper!.Map(product, res);

            await HandleProduct(res);

            if (await _productRepo.UpdateAsync(product) != null)
            {
                res.Photos!.Add(ObjectMapper!.Map<PhotoDto>(photo));
                return res;
            }
            return "Problem adding photo";
        }
        #endregion

        #region CreateOrUpdate
        public async Task<ProductForViewDto?> CreateOrUpdate(long? id, ProductInput input)
        {
            if(id == null)
            {
                Product product = new();
                input.SpecificationId = $",{input.SpecificationId}";
                ObjectMapper!.Map(input, product);
                await _productRepo.InsertAsync(product);

                ProductForViewDto? res = new();
                product.SpecificationId = product.SpecificationId![1..];
                ObjectMapper!.Map(product, res);

                await HandleProduct(res);

                return res;
            }
            else
            {
                Product? product = await _productRepo.GetAsync((long)id);
                if (product == null) return null;
                input.SpecificationId = $",{input.SpecificationId}";
                ObjectMapper!.Map(input, product);

                await _productRepo.UpdateAsync(product);

                ProductForViewDto? res = new();
                product.SpecificationId = product.SpecificationId![1..];
                ObjectMapper.Map(product, res);

                await HandleProduct(res);

                return res;
            }
        }
        #endregion

        #region DeleteProduct
        public async Task<long> Delete(long id)
        {
            await _productRepo.DeleteAsync(id);
            return id;
        }
        #endregion

        #region GetProduct
        public async Task<ProductForViewDto?> GetProduct(long id)
        {
            IQueryable<ProductForViewDto> query = from p in _productRepo.GetAll().AsNoTracking()
                                                  where p.Id == id
                                                  select new ProductForViewDto()
                                                  {
                                                      Id = p.Id,
                                                      Name = p.Name,
                                                      Price = p.Price,
                                                      Description = p.Description,
                                                      ProductCategoryId = p.ProductCategoryId,
                                                      SpecificationId = p.SpecificationId!.Substring(1),
                                                  };
            ProductForViewDto? data = await query.FirstOrDefaultAsync();
            if (data != null)
            {
                await HandleProduct(data);
                return data;
            }
            return null;
        }
        #endregion

        #region GetProductBySpecificationId
        public async Task<ProductForViewDto?> GetProductBySpecificationId(long id, long specificationId)
        {
            IQueryable<ProductForViewDto?> query = from p in _productRepo.GetAll().AsNoTracking()
                                                  where p.Id == id && p.SpecificationId!.Contains(specificationId.ToString())
                                                  select new ProductForViewDto()
                                                  {
                                                      Id = p.Id,
                                                      Name = p.Name,
                                                      Price = p.Price,
                                                      Description = p.Description,
                                                      ProductCategoryId = p.ProductCategoryId,
                                                      SpecificationId = p.SpecificationId!.Substring(1),
                                                  };
            ProductForViewDto? data = await query.SingleOrDefaultAsync();

            if (data != null)
            {
                // Get specification list for product
                data.Specifications = await(from s in _specificationRepo.GetAll().AsNoTracking()
                                            join sc in _specificationCategoryRepo.GetAll().AsNoTracking() on s.SpecificationCategoryId equals sc.Id
                                            where s.Id == specificationId
                                            select new SpecificationDto
                                            {
                                                SpecificationCategoryId = sc.Id,
                                                SpecificationCategoryCode = sc.Code,
                                                Id = s.Id,
                                                Code = s.Code,
                                                Value = s.Value
                                            }).ToListAsync();


                // Get photo list for product
                data.Photos = await(from p in _dataContext.Photo.AsNoTracking()
                                    where p.ProductId == data.Id
                                    select new PhotoDto
                                    {
                                        Id = p.Id,
                                        Url = p.Url,
                                        IsMain = p.IsMain,
                                    }).ToListAsync();

                return data;
            }
            return null;
        }
        #endregion

        #region GetProducts
        public async Task<object> GetProducts(PaginationInput input, ProductFilterDto filter)
        {
            DynamicParameters param = new();

            param.Add("ProductCategoryId", null);
            param.Add("SpecificationId", string.IsNullOrWhiteSpace(filter.SpecificationIds) ? @"""" : filter.SpecificationIds);
            param.Add("Price", filter.Price);
            param.Add("Keyword", string.IsNullOrWhiteSpace(filter.Keyword) ? @"""" : filter.Keyword);

            if (input.PageNum != null && input.PageSize != null)
            {
                PaginationResult<ProductForViewDto> products = await _dapper.GetAllAndPaginationAsync<ProductForViewDto>(@$"
                    select 
			            Id,
			            [Name],
			            Price,
			            [Description],
			            ProductCategoryId,
			            substring(SpecificationId, 2, len(SpecificationId)) SpecificationId
		            from Product 
		            where IsDeleted = 0
			            and (freetext(SpecificationId, @SpecificationId) or @SpecificationId = '""')
			            and (freetext([Name], @Keyword) or @Keyword = '""')
			            or [Name] like '%' + @Keyword +'%'
			            and @Price is null or (Price >= @Price - 1000000 and Price <= @Price + 1000000)
                    ", input, param);

                await HandleProductList(products.Content!);
                return products;
            }
            else
            {
                List<ProductForViewDto> query = await _dapper.GetAllAsync<ProductForViewDto>(@$"
                    select 
			            Id,
			            [Name],
			            Price,
			            [Description],
			            ProductCategoryId,
			            substring(SpecificationId, 2, len(SpecificationId)) SpecificationId
		            from Product 
		            where IsDeleted = 0
			            and (freetext(SpecificationId, @SpecificationId) or @SpecificationId = '""')
			            and (freetext([Name], @Keyword) or @Keyword = '""')
			            or [Name] like '%' + @Keyword +'%'
			            and @Price is null or (Price >= @Price - 1000000 and Price <= @Price + 1000000)
                    ", param, CommandType.Text);
                await HandleProductList(query);
                return query;
            }
        }
        #endregion

        #region RemovePhoto
        public async Task<object?> RemovePhoto(long id, int photoId)
        {
            Product? product = await _productRepo.GetAll().Where(p => p.Id == id).Include(p => p.Photos).FirstOrDefaultAsync();

            if (product == null) return null;

            Photo? photo = product.Photos!.SingleOrDefault(p => p.Id == photoId);
            if (photo == null) return null;

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return result.Error.Message;
            }

            product.Photos!.Remove(photo);

            ProductForViewDto? res = new();
            ObjectMapper!.Map(product, res);

            await HandleProduct(res);


            if (await _productRepo.UpdateAsync(product) != null)
            {
                res.Photos!.Add(ObjectMapper!.Map<PhotoDto>(photo));
                return res;
            }
            return "Failed to delete your photo";
        }
        #endregion

        #region Private method
        private async Task HandleProductList(ICollection<ProductForViewDto> list)
        {
            if (list != null)
            {
                foreach (ProductForViewDto product in list!)
                {
                    await HandleProduct(product);
                }
            }
        }

        private async Task HandleProduct(ProductForViewDto product)
        {
            if (product != null)
            {
                // Get specification list for product
                List<string>? specifications = product.SpecificationId != null ? product.SpecificationId!.Split(",").ToList() : null;
                if (specifications != null)
                {
                    product.Specifications = await (from s in _specificationRepo.GetAll().AsNoTracking()
                                                    join sc in _specificationCategoryRepo.GetAll().AsNoTracking() on s.SpecificationCategoryId equals sc.Id
                                                    where specifications.Contains(s.Id.ToString())
                                                    select new SpecificationDto
                                                    {
                                                        SpecificationCategoryId = sc.Id,
                                                        SpecificationCategoryCode = sc.Code,
                                                        Id = s.Id,
                                                        Code = s.Code,
                                                        Value = s.Value
                                                    }).ToListAsync();
                }

                // Get photo list for product
                product.Photos = await (from p in _dataContext.Photo.AsNoTracking()
                                        where p.ProductId == product.Id
                                        select new PhotoDto
                                        {
                                            Id = p.Id,
                                            Url = p.Url,
                                            IsMain = p.IsMain,
                                        }).ToListAsync();
            }
        }
        #endregion
    }
}
