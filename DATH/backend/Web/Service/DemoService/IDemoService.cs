using Service.DemoService.Dto;

namespace Service.DemoService
{
    public interface IDemoService
    {
        Task<List<DemoDto>> GetAll();
        Task CreateOrUpdate(DemoDto input);
        Task Delete(DemoDto input);
    }
}
