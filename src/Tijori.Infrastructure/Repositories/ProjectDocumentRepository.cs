using Microsoft.EntityFrameworkCore;
using Tijori.Application.Interfaces.Repositories;
using Tijori.Domain.Entities;
using Tijori.Infrastructure.Data;

namespace Tijori.Infrastructure.Repositories;

public class ProjectDocumentRepository : IProjectDocumentRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectDocumentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(IEnumerable<ProjectDocument> documents, CancellationToken cancellationToken = default) =>
        await _context.ProjectDocuments.AddRangeAsync(documents, cancellationToken);
}
