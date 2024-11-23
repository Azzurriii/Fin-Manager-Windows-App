using Fin_Manager_v2.Models;

namespace Fin_Manager_v2.Contracts.Services;

public interface ITagService
{
    Task<List<TagModel>> GetTagsAsync();
    Task<TagModel?> GetTagAsync(int id);
    Task<TagModel> CreateTagAsync(string name);
    Task DeleteTagAsync(int id);
    Task<List<TagModel>> GetTagsByTypeAsync(string type);
}