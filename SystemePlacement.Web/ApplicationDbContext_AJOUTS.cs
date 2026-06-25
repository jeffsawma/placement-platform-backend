// ============================================================
// AJOUTS dans ApplicationDbContext.cs
// ============================================================

    public DbSet<Candidature> Candidatures { get; set; }
    public DbSet<CandidatureDocument> CandidatureDocuments { get; set; }

// ============================================================
// AJOUTS dans Program.cs
// ============================================================

    builder.Services.AddScoped<ICandidatureRepository, CandidatureRepository>();
    builder.Services.AddScoped<ICandidatureService, CandidatureService>();

// ============================================================
// MIGRATION
// ============================================================
//
//   dotnet ef migrations add AjoutCandidatures
//   dotnet ef database update
