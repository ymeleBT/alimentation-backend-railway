namespace AlimentationDongmo.Api.Contracts;

public record PagedResultDto<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize, int TotalPages);
