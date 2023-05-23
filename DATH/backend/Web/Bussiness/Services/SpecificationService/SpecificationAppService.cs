using AutoMapper;
using Bussiness.Dto;
using Bussiness.Helper;
using Bussiness.Interface.SpecificationInterface;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Bussiness.Services.SpecificationService
{
    public class SpecificationAppService : BaseService, ISpecificationAppService
    {
        private readonly IRepository<Specification, long> _specRepo;

        public SpecificationAppService(
            IMapper mapper,
            IRepository<Specification, long> specRepo
            )
        {
            ObjectMapper = mapper;
            _specRepo = specRepo;
        }

        #region CreateOrUpdate
        public async Task<SpecificationForViewDto?> CreateOrUpdate(long? id, SpecificationInput input)
        {
            if (id is null)
            {
                Specification? specification = new();
                ObjectMapper!.Map(input, specification);
                await _specRepo.InsertAsync(specification);

                SpecificationForViewDto? res = new();
                ObjectMapper!.Map(specification, res);
                return res;
            }
            else
            {
                Specification? specification = await _specRepo.GetAsync((int)id);
                if (specification == null) return null;
                ObjectMapper!.Map(input, specification);

                await _specRepo.UpdateAsync(specification!);
                SpecificationForViewDto? res = new();
                ObjectMapper!.Map(specification, res);
                return res;
            }
        }
        #endregion

        #region Delete
        public async Task Delete(long id)
        {
            await _specRepo.DeleteAsync(id);
        }
        #endregion

        #region GetSpecification
        public async Task<SpecificationForViewDto?> GetSpecification(long id)
        {
            IQueryable<SpecificationForViewDto> query = from s in _specRepo.GetAll().AsNoTracking()
                                                        where s.Id == id
                                                        select new SpecificationForViewDto()
                                                        {
                                                            Id = s.Id,
                                                            Code = s.Code,
                                                            Value = s.Value,
                                                            Description = s.Description,
                                                            SpecificationCategoryId = s.SpecificationCategoryId,
                                                        };
            SpecificationForViewDto? data = await query.SingleOrDefaultAsync();
            if (data == null) return null;

            return data;
        }
        #endregion

        #region GetSpecifications
        public async Task<object> GetSpecifications(PaginationInput input)
        {
            IQueryable<SpecificationForViewDto> query = from s in _specRepo.GetAll().AsNoTracking()
                                                        select new SpecificationForViewDto()
                                                        {
                                                            Id = s.Id,
                                                            Code = s.Code,
                                                            Value = s.Value,
                                                            Description = s.Description,
                                                            SpecificationCategoryId = s.SpecificationCategoryId,
                                                        };

            if (input.PageNum != null && input.PageSize != null) return await query.Pagination(input);
            else return await query.ToListAsync();
        }
        #endregion
    }
}
