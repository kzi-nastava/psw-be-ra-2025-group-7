using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public
{
    public interface IBundleService
    {
        List<BundleDto> GetByAuthor(long authorId);
        BundleDto Get(long id, long authorId);

        BundleDto Create(long authorId, CreateBundleDto dto);
        BundleDto Update(long id, long authorId, UpdateBundleDto dto);
        void Delete(long id, long authorId);

        BundlePreviewResponseDto PreviewTotal(long authorId, BundlePreviewRequestDto dto);
    }
}
