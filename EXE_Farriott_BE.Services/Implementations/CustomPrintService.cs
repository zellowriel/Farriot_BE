using EXE_Farriott_BE.Repositories.Interfaces;
using EXE_Farriott_BE.Services.DTOs;
using EXE_Farriott_BE.Services.Interfaces;

namespace EXE_Farriott_BE.Services.Implementations
{
    public class CustomPrintService : ICustomPrintService
    {
        private readonly ICustomPrintRepository _printRepo;

        public CustomPrintService(ICustomPrintRepository printRepo) => _printRepo = printRepo;

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateCustomPrintAsync(int id, UpdateCustomPrintRequest request)
        {
            var print = await _printRepo.GetByIdAsync(id);
            if (print == null) return (false, "Không tìm thấy thiết kế cá nhân hóa.");

            print.UploadedImageUrl = request.UploadedImageUrl;
            print.PrintMetadata = request.PrintMetadata;
            print.CustomerNotes = request.CustomerNotes;

            await _printRepo.UpdateAsync(print);
            return (true, string.Empty);
        }

        public async Task<bool> DeleteCustomPrintAsync(int id)
        {
            var print = await _printRepo.GetByIdAsync(id);
            if (print == null) return false;

            await _printRepo.DeleteAsync(print);
            return true;
        }
    }
}