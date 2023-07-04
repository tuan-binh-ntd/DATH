using AutoMapper;
using Bussiness.Dto;
using Bussiness.Helper;
using Bussiness.Interface.SpecificationCategoryInterface;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Bussiness.Services.SpecificationCategoryService
{
    public class SpecificationCategoryAppService : BaseService, ISpecificationCategoryAppService
    {
        private readonly IRepository<SpecificationCategory> _specCateRepo;
        private readonly IRepository<Specification, long> _specificationRepo;

        public SpecificationCategoryAppService(
            IMapper mapper,
            IRepository<SpecificationCategory> specCateRepo,
            IRepository<Specification, long> specificationRepo
            )
        {
            ObjectMapper = mapper;
            _specCateRepo = specCateRepo;
            _specificationRepo = specificationRepo;
        }

        #region CreateOrUpdate
        public async Task<SpecificationCategoryForViewDto?> CreateOrUpdate(int? id, SpecificationCategoryInput input)
        {
            if (id is null)
            {
                SpecificationCategory specificationCategory = new();
                ObjectMapper!.Map(input, specificationCategory);
                await _specCateRepo.InsertAsync(specificationCategory);
                SpecificationCategoryForViewDto? res = new();
                ObjectMapper!.Map(specificationCategory, res);
                return res;
            }
            else
            {
                SpecificationCategory? specificationCategory = await _specCateRepo.GetAsync((int)id);
                if (specificationCategory == null) return null;
                specificationCategory = ObjectMapper!.Map(input, specificationCategory);
                await _specCateRepo.UpdateAsync(specificationCategory);
                SpecificationCategoryForViewDto? res = new();
                ObjectMapper!.Map(specificationCategory, res);
                return res;
            }
        }
        #endregion

        #region Delete
        public async Task Delete(int id)
        {
            await _specCateRepo.DeleteAsync(id);
        }
        #endregion

        #region GetColorByCode
        public async Task<ICollection<GetColorByCodeForViewDto>?> GetColorByCode(string code)
        {
            SpecificationCategory? data = await _specCateRepo.GetAll().AsNoTracking().Where(sc => sc.Code!.ToLower().Contains(code.ToLower())).SingleOrDefaultAsync();
            if (data != null)
            {
                IQueryable<GetColorByCodeForViewDto> query = from s in _specificationRepo.GetAll().AsNoTracking()
                                                             where s.SpecificationCategoryId == data.Id
                                                             select new GetColorByCodeForViewDto
                                                             {
                                                                 Id = s.Id,
                                                                 Code = s.Code,
                                                                 Value = s.Value,
                                                                 Description = s.Description,
                                                             };
                List<GetColorByCodeForViewDto>? res = await query.ToListAsync();
                if (res == null) return null;
                return res;
            }

            return null;
        }
        #endregion

        #region GetSpecificationCategorys
        public async Task<object> GetSpecificationCategorys(PaginationInput input)
        {
            IQueryable<SpecificationCategoryForViewDto> query = from s in _specCateRepo.GetAll().AsNoTracking()
                                                                select new SpecificationCategoryForViewDto()
                                                                {
                                                                    Id = s.Id,
                                                                    Code = s.Code,
                                                                    Value = s.Value
                                                                };

            if (input.PageNum != null && input.PageSize != null) return await query.Pagination(input);
            else return await query.ToListAsync();
        }
        #endregion

        #region GetSpecificationCategory
        public async Task<SpecificationCategoryForViewDto?> GetSpecificationCategory(int id)
        {
            IQueryable<SpecificationCategoryForViewDto> query = from s in _specCateRepo.GetAll().AsNoTracking()
                                                                where s.Id == id
                                                                select new SpecificationCategoryForViewDto()
                                                                {
                                                                    Id = s.Id,
                                                                    Code = s.Code,
                                                                    Value = s.Value
                                                                };

            SpecificationCategoryForViewDto? data = await query.SingleOrDefaultAsync();
            if (data == null) return null;
            return data;
        }
        #endregion
    }
}
