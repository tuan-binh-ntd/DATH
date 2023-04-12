using AutoMapper;
using Bussiness.Dto;
using Bussiness.Extensions;
using Bussiness.Interface;
using Database;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace API.Controllers
{
    public class AccountController : AdminBaseController
    {
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly DataContext _dataContext;

        //private readonly IRepository<Customer, long> _customerRepo;
        //private readonly IRepository<Employee, long> _employeeRepo;

        public AccountController(
            IMapper mapper,
            ITokenService tokenService,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            DataContext dataContext
            //IRepository<Customer, long> customerRepo,
            //IRepository<Employee, long> employeeRepo
            )
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _dataContext = dataContext;
            //_customerRepo = customerRepo;
            //_employeeRepo = employeeRepo;
        }

        [AllowAnonymous]
        [HttpPost("customers")]
        public async Task<IActionResult> SignUp(RegisterDto registerDto)
        {
            if (await CheckUserExists(registerDto.Username!)) return CustomResult("Username is taken", HttpStatusCode.BadRequest);

            var user = _mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.Username!.ToLower();

            var result = await _userManager.CreateAsync(user, registerDto.Password!);

            if (!result.Succeeded) return CustomResult(result.Errors, HttpStatusCode.BadRequest);

            var roleResult = await _userManager.AddToRoleAsync(user, "Customer");

            if (!roleResult.Succeeded) return CustomResult(result.Errors, HttpStatusCode.BadRequest);

            // insert customer info

            //Customer customer = new();
            //customer.UserId = user.Id;
            //_mapper.Map(registerDto ,customer);

            //await _customerRepo.InsertAsync(customer);

            Customer customer = new()
            {
                UserId = user.Id,
                CreationTime = DateTime.Now
            };
            _mapper.Map(registerDto, customer);

            await _dataContext.Customer.AddAsync(customer);
            await _dataContext.SaveChangesAsync();
            UserDto userDto = new();
            _mapper.Map(customer, userDto);
            return CustomResult(userDto);
        }

        [HttpPost("employees")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (await CheckUserExists(registerDto.Username!)) return CustomResult("Username is taken", HttpStatusCode.BadRequest);

            var user = _mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.Username!.ToLower();

            var result = await _userManager.CreateAsync(user, registerDto.Password!);

            if (!result.Succeeded) return CustomResult(result.Errors, HttpStatusCode.BadRequest);

            var roleResult = await _userManager.AddToRoleAsync(user, "Employee");

            if (!roleResult.Succeeded) return CustomResult(result.Errors, HttpStatusCode.BadRequest);

            // insert employee info

            //Employee employee = new();
            //employee.UserId = user.Id;
            //_mapper.Map(registerDto, employee);

            //await _employeeRepo.InsertAsync(employee);


            Employee employee = new()
            {
                UserId = user.Id,
                CreationTime = DateTime.Now,
                CreatorUserId = User.GetUserId()
            };
            _mapper.Map(registerDto, employee);

            await _dataContext.Employee.AddAsync(employee);
            await _dataContext.SaveChangesAsync();
            UserDto userDto = new();
            _mapper.Map(employee, userDto);
            return CustomResult(userDto);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username!.ToLower());

            if (user == null) return CustomResult("Invalid username", HttpStatusCode.Unauthorized);

            var result = await _signInManager
                .CheckPasswordSignInAsync(user, loginDto.Password!, false);

            if (!result.Succeeded) return CustomResult(HttpStatusCode.Unauthorized);

            UserDto res = new()
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user)
            };

            //Customer? customer = await _customerRepo.GetAll().AsNoTracking().Where(c => c.UserId == user.Id).FirstOrDefaultAsync();

            //if(customer == null)
            //{
            //    var employee = await _employeeRepo.GetAll().AsNoTracking().Where(c => c.UserId == user.Id).FirstOrDefaultAsync();
            //    _mapper.Map(res, employee);

            //    return CustomResult(res);
            //}
            //_mapper.Map(customer, res);

            //Customer? customer = await _customerRepo.GetAll().AsNoTracking().Where(c => c.UserId == user.Id).FirstOrDefaultAsync();

            Customer? customer = await _dataContext.Customer.AsNoTracking().Where(c => c.UserId == user.Id).FirstOrDefaultAsync();
            if (customer == null)
            {
                Employee? employee = await _dataContext.Employee.AsNoTracking().Where(c => c.UserId == user.Id).FirstOrDefaultAsync();
                _mapper.Map(res, employee);

                return CustomResult(res);
            }
            _mapper.Map(customer, res);

            return CustomResult(res);
        }

        private async Task<bool> CheckUserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }      
    }
}
