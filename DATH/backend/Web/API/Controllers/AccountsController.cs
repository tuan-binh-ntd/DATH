using AutoMapper;
using Bussiness.Dto;
using Bussiness.Interface;
using Bussiness.Repository;
using Entities;
using Entities.Enum.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace API.Controllers
{
    public class AccountsController : AdminBaseController
    {
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IRepository<Customer, long> _customerRepo;
        private readonly IRepository<Employee, long> _employeeRepo;
        private readonly IPhotoService _photoService;
        private readonly ISession _session;

        public AccountsController(
            IMapper mapper,
            ITokenService tokenService,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IRepository<Customer, long> customerRepo,
            IRepository<Employee, long> employeeRepo,
            IPhotoService photoService,
            ISession session
            )
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _customerRepo = customerRepo;
            _employeeRepo = employeeRepo;
            _photoService = photoService;
            _session = session;
        }

        [AllowAnonymous]
        [HttpPost("customers")]
        public async Task<IActionResult> SignUp(RegisterDto registerDto)
        {
            if (await CheckUserExists(registerDto.Username!)) return CustomResult("Username is taken", HttpStatusCode.BadRequest);

            var user = _mapper.Map<AppUser>(registerDto);
            user.Type = UserType.Customer;
            user.UserName = registerDto.Username!.ToLower();

            var result = await _userManager.CreateAsync(user, registerDto.Password!);

            if (!result.Succeeded) return CustomResult(result.Errors, HttpStatusCode.BadRequest);

            var roleResult = await _userManager.AddToRoleAsync(user, "Customer");

            if (!roleResult.Succeeded) return CustomResult(result.Errors, HttpStatusCode.BadRequest);

            // insert customer info

            Customer customer = new()
            {
                UserId = user.Id
            };
            _mapper.Map(registerDto, customer);

            await _customerRepo.InsertAsync(customer);

            UserDto userDto = new()
            {
                Token = await _tokenService.CreateToken(user)
            };
            _mapper.Map(customer, userDto);
            return CustomResult(userDto, HttpStatusCode.OK);
        }

        [HttpPost("employees")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (await CheckUserExists(registerDto.Username!)) return CustomResult("Username is taken", HttpStatusCode.BadRequest);

            var user = _mapper.Map<AppUser>(registerDto);
            user.Type = UserType.Employee;
            user.UserName = registerDto.Username!.ToLower();

            var result = await _userManager.CreateAsync(user, registerDto.Password!);

            if (!result.Succeeded) return CustomResult(result.Errors, HttpStatusCode.BadRequest);

            var roleResult = await _userManager.AddToRoleAsync(user, "Employee");

            if (!roleResult.Succeeded) return CustomResult(result.Errors, HttpStatusCode.BadRequest);

            // insert employee info

            Employee employee = new()
            {
                UserId = user.Id,
            };
            _mapper.Map(registerDto, employee);

            await _employeeRepo.InsertAsync(employee);

            _mapper.Map(registerDto, employee);

            UserDto userDto = new()
            {
                Token = await _tokenService.CreateToken(user)
            };
            _mapper.Map(employee, userDto);
            return CustomResult(userDto, HttpStatusCode.OK);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username!.ToLower());

            if (user == null) return CustomResult("Invalid username", HttpStatusCode.Unauthorized);

            var result = await _signInManager
                .CheckPasswordSignInAsync(user, loginDto.Password!, false);

            if (!result.Succeeded) return CustomResult("Incorrect name or password", HttpStatusCode.Unauthorized);

            _session.SetString("UserId", user.Id.ToString());

            UserDto res = new()
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user)
            };

            Customer? customer = await _customerRepo.GetAll().AsNoTracking().Where(c => c.UserId == user.Id).FirstOrDefaultAsync();

            if (customer == null)
            {
                Employee? employee = await _employeeRepo.GetAll().AsNoTracking().Where(c => c.UserId == user.Id).FirstOrDefaultAsync();
                _mapper.Map(res, employee);

                return CustomResult(res);
            }
            _mapper.Map(customer, res);


            _mapper.Map(customer, res);

            return CustomResult(res, HttpStatusCode.OK);
        }

        private async Task<bool> CheckUserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

        [HttpPut("{id}/photos")]
        public async Task<IActionResult> UploadPhoto(long id, IFormFile file)
        {
            AppUser? user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return CustomResult(HttpStatusCode.NotFound);

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            user.AvatarUrl = result.SecureUrl.AbsoluteUri;
            user.PublicId = result.PublicId;

            await _userManager.UpdateAsync(user);

            return CustomResult(HttpStatusCode.OK);
        }

        [HttpGet("{username}"), AllowAnonymous]
        public async Task<IActionResult> CheckUsername(string username)
        {
            if (await CheckUserExists(username)) return CustomResult(new { Invalid = false }, HttpStatusCode.OK);
            return CustomResult(new { Invalid = true }, HttpStatusCode.OK);
        }
    }
}
