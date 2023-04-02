using AutoMapper;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using Service.DemoService;
using Service.DemoService.Dto;

namespace Bussiness
{
    public class DemoService : IDemoService
    {
        private readonly IRepository<Demo, long> _demoRepo;
        private readonly IMapper _mapper;

        public DemoService(
            IRepository<Demo, long> demoRepo,
            IMapper mapper
            )
        {
            _demoRepo = demoRepo;
            _mapper = mapper;
        }

        public async Task CreateOrUpdate(DemoDto input)
        {
            if(input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        private async Task Create(DemoDto input)
        {
            Demo data = _mapper.Map<Demo>(input);
            await _demoRepo.InsertAsync(data);
        }

        private async Task Update(DemoDto input)
        {
            Demo data = await _demoRepo.GetAsync((long)input.Id);
            data = _mapper.Map<Demo>(input);
            await _demoRepo.UpdateAsync(data);
        }

        public async Task Delete(DemoDto input)
        {
            await _demoRepo.DeleteAsync((long)input.Id);
        }

        public async Task<List<DemoDto>> GetAll()
        {
            IQueryable<DemoDto> query = from d in _demoRepo.GetAll().AsNoTracking()
                                        select new DemoDto
                                        {
                                            Name = d.Name,
                                        };
            return await query.ToListAsync();
        }
    }
}