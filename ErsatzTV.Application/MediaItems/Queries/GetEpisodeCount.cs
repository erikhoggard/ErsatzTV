using ErsatzTV.Core;

namespace ErsatzTV.Application.MediaItems;

public record GetEpisodeCount(int ShowId) : IRequest<int>;
