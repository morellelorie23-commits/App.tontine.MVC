namespace tontine.MVC.Services
{
    public class CommunicationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IEmailService _email;
        private readonly ILogger<CommunicationBackgroundService> _logger;

        public CommunicationBackgroundService(
            IServiceScopeFactory scopeFactory,
            IEmailService email,
            ILogger<CommunicationBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _email        = email;
            _logger       = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Attendre 30 s au démarrage pour laisser l'app se stabiliser
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try   { await EnvoyerRappels(stoppingToken); }
                catch (Exception ex) { _logger.LogError(ex, "Erreur dans CommunicationBackgroundService"); }

                // Relancer toutes les 24 h
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task EnvoyerRappels(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var cotisations = scope.ServiceProvider.GetRequiredService<ICotisationService>();
            var membres     = scope.ServiceProvider.GetRequiredService<IMembreService>();

            var toutes = await cotisations.GetAllAsync();
            var limite = DateTime.Today.AddDays(3);

            // Cotisations en attente dont l'échéance est dans 1 à 3 jours
            var aRappeler = toutes.Where(c =>
                c.Statut == "En attente" &&
                c.DateCotisation.Date >= DateTime.Today &&
                c.DateCotisation.Date <= limite).ToList();

            _logger.LogInformation("Rappels cotisation : {N} à envoyer", aRappeler.Count);

            var membreCache = new Dictionary<int, string?>();

            foreach (var cot in aRappeler)
            {
                if (ct.IsCancellationRequested) break;

                if (!membreCache.TryGetValue(cot.IdMembre, out var email))
                {
                    var m = await membres.GetByIdAsync(cot.IdMembre);
                    email = m?.Email;
                    membreCache[cot.IdMembre] = email;
                }

                if (string.IsNullOrEmpty(email)) continue;

                var m2 = await membres.GetByIdAsync(cot.IdMembre);
                var nom = m2 != null ? $"{m2.Prenom} {m2.Nom}" : "Membre";

                await _email.SendRappelCotisationAsync(email, nom, cot.LibelleTontine, cot.Montant, cot.DateCotisation);
                _logger.LogInformation("Rappel envoyé → {Email} ({Tontine})", email, cot.LibelleTontine);
            }
        }
    }
}
