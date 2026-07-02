using ClosedXML.Excel;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class ExcelService
    {
        private static void StylerEntete(IXLRow row, int colonnes)
        {
            var range = row.Worksheet.Range(row.RowNumber(), 1, row.RowNumber(), colonnes);
            range.Style.Fill.BackgroundColor = XLColor.FromHtml("#0F6E56");
            range.Style.Font.FontColor = XLColor.White;
            range.Style.Font.Bold = true;
        }

        private static void StylerLignes(IXLWorksheet ws, int debut, int fin, int colonnes)
        {
            for (int i = debut; i <= fin; i++)
            {
                var range = ws.Range(i, 1, i, colonnes);
                if (i % 2 == 0)
                    range.Style.Fill.BackgroundColor = XLColor.FromHtml("#F4F6F9");
                range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                range.Style.Border.BottomBorderColor = XLColor.FromHtml("#E0E0E0");
            }
        }

        private static void EnteteRapport(IXLWorksheet ws, string titre, string nomCycle)
        {
            ws.Cell(1, 1).Value = $"{titre} — Cycle : {nomCycle}";
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 14;
            ws.Cell(2, 1).Value = $"Exporté le {DateTime.Now:dd/MM/yyyy HH:mm}";
            ws.Cell(2, 1).Style.Font.Italic = true;
            ws.Cell(2, 1).Style.Font.FontColor = XLColor.Gray;
        }

        public byte[] ExporterCotisations(List<CotisationViewModel> donnees, string nomCycle)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Cotisations");
            EnteteRapport(ws, "Cotisations", nomCycle);

            var entetes = new[] { "Membre", "Tontine", "Montant (FCFA)", "Mode Paiement", "Date", "Statut", "Mandataire" };
            for (int c = 1; c <= entetes.Length; c++) ws.Row(4).Cell(c).Value = entetes[c - 1];
            StylerEntete(ws.Row(4), entetes.Length);

            int l = 5;
            foreach (var c in donnees)
            {
                ws.Cell(l, 1).Value = c.NomMembre;
                ws.Cell(l, 2).Value = c.LibelleTontine;
                ws.Cell(l, 3).Value = c.Montant;
                ws.Cell(l, 3).Style.NumberFormat.Format = "#,##0";
                ws.Cell(l, 4).Value = c.ModePaiement;
                ws.Cell(l, 5).Value = c.DateCotisation.ToString("dd/MM/yyyy");
                ws.Cell(l, 6).Value = c.Statut;
                ws.Cell(l, 7).Value = c.NomMandataire ?? "";
                l++;
            }
            StylerLignes(ws, 5, l - 1, entetes.Length);

            ws.Cell(l + 1, 2).Value = "TOTAL";
            ws.Cell(l + 1, 2).Style.Font.Bold = true;
            ws.Cell(l + 1, 3).Value = donnees.Sum(c => c.Montant);
            ws.Cell(l + 1, 3).Style.NumberFormat.Format = "#,##0";
            ws.Cell(l + 1, 3).Style.Font.Bold = true;
            ws.Cell(l + 1, 3).Style.Font.FontColor = XLColor.FromHtml("#0F6E56");

            ws.Columns().AdjustToContents();
            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        public byte[] ExporterVersements(List<VersementViewModel> donnees, string nomCycle)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Versements");
            EnteteRapport(ws, "Versements", nomCycle);

            var entetes = new[] { "Membre", "Tontine", "Montant Brut", "Commission", "Déduction", "Montant Net", "Date", "Notes" };
            for (int c = 1; c <= entetes.Length; c++) ws.Row(4).Cell(c).Value = entetes[c - 1];
            StylerEntete(ws.Row(4), entetes.Length);

            int l = 5;
            foreach (var v in donnees)
            {
                ws.Cell(l, 1).Value = v.NomMembre;
                ws.Cell(l, 2).Value = v.LibelleTontine;
                ws.Cell(l, 3).Value = v.Montant;
                ws.Cell(l, 4).Value = v.MontantCommission;
                ws.Cell(l, 5).Value = v.MontantDeduction;
                ws.Cell(l, 6).Value = v.MontantNet;
                ws.Cell(l, 7).Value = v.DateVersement.ToString("dd/MM/yyyy");
                ws.Cell(l, 8).Value = v.Notes ?? "";
                for (int c = 3; c <= 6; c++) ws.Cell(l, c).Style.NumberFormat.Format = "#,##0";
                l++;
            }
            StylerLignes(ws, 5, l - 1, entetes.Length);

            ws.Cell(l + 1, 2).Value = "TOTAL NET";
            ws.Cell(l + 1, 2).Style.Font.Bold = true;
            ws.Cell(l + 1, 6).Value = donnees.Sum(v => v.MontantNet);
            ws.Cell(l + 1, 6).Style.NumberFormat.Format = "#,##0";
            ws.Cell(l + 1, 6).Style.Font.Bold = true;
            ws.Cell(l + 1, 6).Style.Font.FontColor = XLColor.FromHtml("#0F6E56");

            ws.Columns().AdjustToContents();
            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        public byte[] ExporterPrets(List<PretViewModel> donnees, string nomCycle)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Prêts");
            EnteteRapport(ws, "Prêts", nomCycle);

            var entetes = new[] { "Membre", "Montant (FCFA)", "Taux (%)", "Date prêt", "Échéance", "Remboursé", "Statut" };
            for (int c = 1; c <= entetes.Length; c++) ws.Row(4).Cell(c).Value = entetes[c - 1];
            StylerEntete(ws.Row(4), entetes.Length);

            int l = 5;
            foreach (var p in donnees)
            {
                ws.Cell(l, 1).Value = p.NomMembre ?? "";
                ws.Cell(l, 2).Value = p.Montant;
                ws.Cell(l, 2).Style.NumberFormat.Format = "#,##0";
                ws.Cell(l, 3).Value = p.Taux;
                ws.Cell(l, 4).Value = p.DatePret.ToString("dd/MM/yyyy");
                ws.Cell(l, 5).Value = p.DateRemboursement.ToString("dd/MM/yyyy");
                ws.Cell(l, 6).Value = p.MontantRemboursé;
                ws.Cell(l, 6).Style.NumberFormat.Format = "#,##0";
                ws.Cell(l, 7).Value = p.Statut;
                l++;
            }
            StylerLignes(ws, 5, l - 1, entetes.Length);

            ws.Cell(l + 1, 1).Value = "TOTAL PRÊTS";
            ws.Cell(l + 1, 1).Style.Font.Bold = true;
            ws.Cell(l + 1, 2).Value = donnees.Sum(p => p.Montant);
            ws.Cell(l + 1, 2).Style.NumberFormat.Format = "#,##0";
            ws.Cell(l + 1, 2).Style.Font.Bold = true;
            ws.Cell(l + 1, 2).Style.Font.FontColor = XLColor.FromHtml("#0F6E56");

            ws.Columns().AdjustToContents();
            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        public byte[] ExporterRapport(RapportViewModel rapport, string nomCycle)
        {
            using var wb = new XLWorkbook();

            // Feuille synthèse
            var ws = wb.Worksheets.Add("Synthèse");
            EnteteRapport(ws, "Rapport financier", nomCycle);

            var lignes = new (string, decimal)[]
            {
                ("Total cotisations collectées", rapport.TotalCotisations),
                ("Total versements effectués",   rapport.TotalVersements),
                ("Total remboursé (prêts)",      rapport.TotalRembourse),
                ("Total prêts accordés",         rapport.TotalPrets),
                ("Solde estimé",                 rapport.SoldeActuel)
            };

            int l = 4;
            foreach (var (libelle, montant) in lignes)
            {
                ws.Cell(l, 1).Value = libelle;
                ws.Cell(l, 2).Value = montant;
                ws.Cell(l, 2).Style.NumberFormat.Format = "#,##0 FCFA";
                ws.Cell(l, 2).Style.Font.Bold = true;
                l++;
            }

            ws.Cell(l + 1, 1).Value = "Statistiques";
            ws.Cell(l + 1, 1).Style.Font.Bold = true;
            ws.Cell(l + 1, 1).Style.Font.FontSize = 12;
            l += 2;

            var stats = new (string, object)[]
            {
                ("Cotisations payées",   rapport.CotisationsPayees),
                ("Cotisations en retard", rapport.CotisationsEnRetard),
                ("Taux de paiement",     $"{rapport.TauxPaiement:0.0}%"),
                ("Prêts approuvés",      rapport.PretsApprouves),
                ("Prêts en retard",      rapport.PretsEnRetard),
            };
            foreach (var (libelle, valeur) in stats)
            {
                ws.Cell(l, 1).Value = libelle;
                ws.Cell(l, 2).Value = valeur.ToString();
                l++;
            }

            ws.Columns().AdjustToContents();

            // Feuille top contributeurs
            if (rapport.TopMembres?.Any() == true)
            {
                var wsTop = wb.Worksheets.Add("Top Membres");
                EnteteRapport(wsTop, "Top Contributeurs", nomCycle);

                var entetes = new[] { "Rang", "Membre", "Total cotisé (FCFA)" };
                for (int c = 1; c <= entetes.Length; c++) wsTop.Row(4).Cell(c).Value = entetes[c - 1];
                StylerEntete(wsTop.Row(4), entetes.Length);

                int ligne = 5, rang = 1;
                foreach (var kv in rapport.TopMembres.OrderByDescending(x => x.Value))
                {
                    wsTop.Cell(ligne, 1).Value = rang++;
                    wsTop.Cell(ligne, 2).Value = kv.Key;
                    wsTop.Cell(ligne, 3).Value = kv.Value;
                    wsTop.Cell(ligne, 3).Style.NumberFormat.Format = "#,##0";
                    ligne++;
                }
                StylerLignes(wsTop, 5, ligne - 1, entetes.Length);
                wsTop.Columns().AdjustToContents();
            }

            // Feuille évolution mensuelle
            if (rapport.EvolutionMensuelle?.Any() == true)
            {
                var wsEvo = wb.Worksheets.Add("Évolution mensuelle");
                EnteteRapport(wsEvo, "Évolution mensuelle", nomCycle);

                var entetes = new[] { "Mois", "Cotisations (FCFA)", "Versements (FCFA)" };
                for (int c = 1; c <= entetes.Length; c++) wsEvo.Row(4).Cell(c).Value = entetes[c - 1];
                StylerEntete(wsEvo.Row(4), entetes.Length);

                int ligne = 5;
                foreach (var evo in rapport.EvolutionMensuelle)
                {
                    wsEvo.Cell(ligne, 1).Value = evo.Mois;
                    wsEvo.Cell(ligne, 2).Value = evo.Cotisations;
                    wsEvo.Cell(ligne, 2).Style.NumberFormat.Format = "#,##0";
                    wsEvo.Cell(ligne, 3).Value = evo.Versements;
                    wsEvo.Cell(ligne, 3).Style.NumberFormat.Format = "#,##0";
                    ligne++;
                }
                StylerLignes(wsEvo, 5, ligne - 1, entetes.Length);
                wsEvo.Columns().AdjustToContents();
            }

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }
    }
}
