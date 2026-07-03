using SystemePlacement.Web.DTOs.Stages;

namespace SystemePlacement.Web.Services.Interfaces;

public interface IStageService
{
    Task<StageResponseDto> CreerStageAsync(StageCreateDto request);

    Task<StageResponseDto?> GetStageByIdAsync(int idStage);

    Task<IReadOnlyList<StageResponseDto>> GetStagesAsync();

    Task<StageResponseDto?> ConfirmerStageAsync(
        int idStage,
        ConfirmationStageCreateDto request);
}