using AutoMapper;
using Bussiness.Dto;
using Bussiness.Helper;
using Bussiness.Interface.InstallmentInterface;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Bussiness.Services.InstallmentService
{
    public class InstallmentAppService : BaseService, IInstallmentAppService
    {
        private readonly IRepository<Installment> _installmentRepo;

        public InstallmentAppService(
             IRepository<Installment> installmentRepo,
            IMapper mapper
            )
        {
            ObjectMapper = mapper;
            _installmentRepo = installmentRepo;
        }

        #region CreateOrUpdate
        public async Task<InstallmentForViewDto?> CreateOrUpdate(int? id, InstallmentInput input)
        {
            if (id is null)
            {
                Installment? data = new();
                ObjectMapper!.Map(input, data);
                await _installmentRepo.InsertAsync(data);

                InstallmentForViewDto? res = new();
                ObjectMapper!.Map(data, res);
                return res;
            }
            else
            {
                Installment? data = await _installmentRepo.GetAsync((int)id);
                if (data == null) return null;
                ObjectMapper!.Map(input, data);

                await _installmentRepo.UpdateAsync(data);
                InstallmentForViewDto? res = new();
                ObjectMapper!.Map(data, res);
                return res;
            }
        }
        #endregion

        #region Delete
        public async Task Delete(int id)
        {
            await _installmentRepo.DeleteAsync(id);
        }
        #endregion

        #region GetInstallment
        public async Task<InstallmentForViewDto?> GetInstallment(int id)
        {
            IQueryable<InstallmentForViewDto> query = from i in _installmentRepo.GetAll().AsNoTracking()
                                                      where i.Id == id
                                                      select new InstallmentForViewDto()
                                                      {
                                                          Id = i.Id,
                                                          Balance = i.Balance,
                                                          Term = i.Term
                                                      };
            InstallmentForViewDto? data = await query.SingleOrDefaultAsync();
            if (data == null) return null;

            return data;
        }
        #endregion

        #region GetInstallments
        public async Task<object> GetInstallments(PaginationInput input)
        {
            IQueryable<InstallmentForViewDto> query = from i in _installmentRepo.GetAll().AsNoTracking()
                                                      select new InstallmentForViewDto()
                                                      {
                                                          Id = i.Id,
                                                          Balance = i.Balance,
                                                          Term = i.Term
                                                      };

            if (input.PageNum != null && input.PageSize != null) return await query.Pagination(input);
            else return await query.ToListAsync();
        }
        #endregion
    }
}
