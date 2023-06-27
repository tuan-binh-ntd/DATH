using AutoMapper;
using Bussiness.Dto;
using Bussiness.Helper;
using Bussiness.Interface.EmployeeInterface;
using Bussiness.Repository;
using Bussiness.Services.Core;
using Entities;
using Entities.Enum.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bussiness.Services.EmployeeService
{
    public class EmployeeAppService : BaseService, IEmployeeAppService
    {
        private readonly IRepository<Shop> _shopRepo;
        private readonly IRepository<Employee, long> _employeeRepo;
        private readonly UserManager<AppUser> _userManager;

        public EmployeeAppService(
            IMapper mapper,
            IRepository<Shop> shopRepo,
            IRepository<Employee, long> employeeRepo,
            UserManager<AppUser> userManager
            )
        {
            ObjectMapper = mapper;
            _shopRepo = shopRepo;
            _employeeRepo = employeeRepo;
            _userManager = userManager;
        }

        #region Delete
        public async Task Delete(long id)
        {
            await _employeeRepo.DeleteAsync(id);
        }
        #endregion

        #region GetEmployees
        public async Task<object> GetEmployees(PaginationInput input)
        {
            IQueryable<EmployeeForViewDto> query = from s in _shopRepo.GetAll().AsNoTracking()
                                                    join e in _employeeRepo.GetAll().AsNoTracking() on s.Id equals e.ShopId
                                                    join u in _userManager.Users on e.UserId equals u.Id
                                                    select new EmployeeForViewDto()
                                                    {
                                                        Id = e.Id,
                                                        ShopName = s.Name,
                                                        ShopId = s.Id,
                                                        FirstName = e.FirstName,
                                                        LastName = e.LastName,
                                                        Code = e.Code,
                                                        Gender = e.Gender,
                                                        Birthday = e.Birthday,
                                                        Type = e.Type,
                                                        Email = e.Email,
                                                        JoinDate = e.JoinDate,
                                                        Address = e.Address,
                                                        IsActive = e.IsActive,
                                                        Username = u.UserName
                                                    };

            if (input.PageNum != null && input.PageSize != null) return await query.Pagination(input);
            else return await query.ToListAsync();
        }
        #endregion

        #region Update
        public async Task<EmployeeForViewDto?> Update(long id, EmployeeInput input)
        {
            Employee? employee = await _employeeRepo.GetAsync(id);
            if (employee == null) return null;
            ObjectMapper!.Map(input, employee);
            await _employeeRepo.UpdateAsync(employee);

            IQueryable<EmployeeForViewDto> query = from s in _shopRepo.GetAll().AsNoTracking()
                                                   join e in _employeeRepo.GetAll().AsNoTracking() on s.Id equals e.ShopId
                                                   join u in _userManager.Users on e.UserId equals u.Id
                                                   where e.Id == id
                                                   select new EmployeeForViewDto()
                                                   {
                                                       Id = e.Id,
                                                       ShopName = s.Name,
                                                       ShopId = s.Id,
                                                       FirstName = e.FirstName,
                                                       LastName = e.LastName,
                                                       Code = e.Code,
                                                       Gender = e.Gender,
                                                       Birthday = e.Birthday,
                                                       Type = e.Type,
                                                       Email = e.Email,
                                                       JoinDate = e.JoinDate,
                                                       Address = e.Address,
                                                       IsActive = e.IsActive,
                                                       Username = u.UserName
                                                   };

            return await query.SingleOrDefaultAsync();
        }
        #endregion

        #region GetEmployee
        public async Task<EmployeeForViewDto?> GetOrderEmployeeByShop(int shopId)
        {
            IQueryable<EmployeeForViewDto> query = from s in _shopRepo.GetAll().AsNoTracking()
                                                   join e in _employeeRepo.GetAll().AsNoTracking() on s.Id equals e.ShopId
                                                   join u in _userManager.Users on e.UserId equals u.Id
                                                   where e.ShopId == shopId && e.Type == EmployeeType.Orders
                                                   select new EmployeeForViewDto()
                                                   {
                                                       Id = e.Id,
                                                       ShopName = s.Name,
                                                       ShopId = s.Id,
                                                       FirstName = e.FirstName,
                                                       LastName = e.LastName,
                                                       Code = e.Code,
                                                       Gender = e.Gender,
                                                       Birthday = e.Birthday,
                                                       Type = e.Type,
                                                       Email = e.Email,
                                                       JoinDate = e.JoinDate,
                                                       Address = e.Address,
                                                       IsActive = e.IsActive,
                                                       Username = u.UserName
                                                   };

            return await query.FirstOrDefaultAsync();
        }
        #endregion
    }
}
