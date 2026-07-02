using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class PdfService
    {
        public byte[] GenererReleveMembrePdf(ReleveMembreViewModel releve)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20, Unit.Millimetre);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    // En-tête
                    page.Header().Element(ComposeHeader);

                    // Contenu
                    page.Content().Column(col =>
                    {
                        col.Spacing(12);

                        // Fiche membre
                        col.Item().Element(c => ComposeFicheMembre(c, releve.Membre));

                        // Résumé financier
                        col.Item().Element(c => ComposeResume(c, releve));

                        // Cotisations
                        if (releve.Cotisations.Any())
                            col.Item().Element(c => ComposeCotisations(c, releve.Cotisations, releve.TotalCotisations));

                        // Versements
                        if (releve.Versements.Any())
                            col.Item().Element(c => ComposeVersements(c, releve.Versements, releve.TotalVersements));

                        // Prêts
                        if (releve.Prets.Any())
                            col.Item().Element(c => ComposePrets(c, releve.Prets, releve.TotalPrets));
                    });

                    // Pied de page
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("TontineApp — Document confidentiel — Édité le ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.Span(DateTime.Now.ToString("dd/MM/yyyy à HH:mm")).FontSize(8).FontColor(Colors.Grey.Medium);
                        x.Span("   Page ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                        x.Span(" / ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });
            }).GeneratePdf();
        }

        public byte[] GenererRecuCotisationPdf(CotisationViewModel c)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(16, Unit.Millimetre);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Element(ComposeHeader);

                    page.Content().Column(col =>
                    {
                        col.Spacing(10);

                        col.Item().Background("#0F6E56").Padding(14).Column(t =>
                        {
                            t.Item().Text("REÇU DE COTISATION").FontColor(Colors.White).Bold().FontSize(14);
                            t.Item().Text($"N° {c.IdCotisation:D5}").FontColor(Colors.White).FontSize(10);
                        });

                        col.Item().Border(1).BorderColor("#E0E0E0").Padding(12).Column(info =>
                        {
                            info.Item().Text("MEMBRE").Bold().FontSize(9).FontColor("#0F6E56");
                            info.Item().PaddingTop(4).Text(c.NomMembre).FontSize(13).Bold();
                        });

                        col.Item().Border(1).BorderColor("#E0E0E0").Padding(12).Column(det =>
                        {
                            det.Item().Text("DÉTAILS").Bold().FontSize(9).FontColor("#0F6E56");
                            det.Item().PaddingTop(8).Table(table =>
                            {
                                table.ColumnsDefinition(cols => { cols.RelativeColumn(); cols.RelativeColumn(2); });
                                table.Cell().Padding(3).Text("Tontine").FontSize(9).FontColor(Colors.Grey.Darken1);
                                table.Cell().Padding(3).Text(c.LibelleTontine).Bold().FontSize(9);
                                table.Cell().Padding(3).Text("Cycle").FontSize(9).FontColor(Colors.Grey.Darken1);
                                table.Cell().Padding(3).Text(c.NomCycle).Bold().FontSize(9);
                                table.Cell().Padding(3).Text("Date").FontSize(9).FontColor(Colors.Grey.Darken1);
                                table.Cell().Padding(3).Text(c.DateCotisation.ToString("dd/MM/yyyy")).Bold().FontSize(9);
                                table.Cell().Padding(3).Text("Statut").FontSize(9).FontColor(Colors.Grey.Darken1);
                                table.Cell().Padding(3).Text(c.Statut).Bold().FontSize(9);
                            });
                        });

                        col.Item().Background("#E6F4EE").Padding(16).Row(row =>
                        {
                            row.RelativeItem().Text("MONTANT").Bold().FontSize(12).FontColor("#0F6E56");
                            row.AutoItem().Text($"{c.Montant:N0} FCFA").Bold().FontSize(16).FontColor("#0F6E56");
                        });

                        col.Item().PaddingTop(12).AlignCenter()
                            .Text($"Document émis le {DateTime.Now:dd/MM/yyyy à HH:mm}")
                            .FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });
            }).GeneratePdf();
        }

        public byte[] GenererRapportPdf(RapportViewModel r)
        {
            string F(decimal v) => v.ToString("N0") + " FCFA";

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20, Unit.Millimetre);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().BorderBottom(1).BorderColor("#0F6E56").PaddingBottom(10).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("TontineApp").FontSize(18).FontColor("#0F6E56").Bold();
                            col.Item().Text("Rapport détaillé").FontSize(11).FontColor(Colors.Grey.Darken1);
                        });
                        row.ConstantItem(160).AlignRight().Column(col =>
                        {
                            col.Item().AlignRight().Text("RAPPORT OFFICIEL").FontSize(8).Bold().FontColor("#0F6E56");
                            col.Item().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy")).FontSize(8).FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Content().Column(col =>
                    {
                        col.Spacing(14);

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Padding(4).Background("#E6F4EE").Padding(10).Column(c =>
                            {
                                c.Item().Text("COTISATIONS").FontSize(8).FontColor("#0F6E56").Bold();
                                c.Item().Text(F(r.TotalCotisations)).FontSize(11).Bold().FontColor("#0F6E56");
                            });
                            row.RelativeItem().Padding(4).Background("#E3F2FD").Padding(10).Column(c =>
                            {
                                c.Item().Text("VERSEMENTS").FontSize(8).FontColor("#1565C0").Bold();
                                c.Item().Text(F(r.TotalVersements)).FontSize(11).Bold().FontColor("#1565C0");
                            });
                            row.RelativeItem().Padding(4).Background("#FFF8E1").Padding(10).Column(c =>
                            {
                                c.Item().Text("PRÊTS").FontSize(8).FontColor("#F57F17").Bold();
                                c.Item().Text(F(r.TotalPrets)).FontSize(11).Bold().FontColor("#F57F17");
                            });
                            row.RelativeItem().Padding(4).Background("#F3E5F5").Padding(10).Column(c =>
                            {
                                c.Item().Text("REMBOURSÉ").FontSize(8).FontColor("#6A1B9A").Bold();
                                c.Item().Text(F(r.TotalRembourse)).FontSize(11).Bold().FontColor("#6A1B9A");
                            });
                        });

                        col.Item().Background("#0F6E56").Padding(8).Text("STATISTIQUES GÉNÉRALES").FontColor(Colors.White).Bold().FontSize(10);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c => { c.RelativeColumn(); c.ConstantColumn(100); });
                            table.Cell().BorderBottom(1).BorderColor("#EEE").Padding(6).Text("Membres").FontSize(9);
                            table.Cell().BorderBottom(1).BorderColor("#EEE").Padding(6).AlignRight().Text(r.NbMembres.ToString()).FontSize(9).Bold();
                            table.Cell().BorderBottom(1).BorderColor("#EEE").Padding(6).Text("Cycles").FontSize(9);
                            table.Cell().BorderBottom(1).BorderColor("#EEE").Padding(6).AlignRight().Text(r.NbCycles.ToString()).FontSize(9).Bold();
                            table.Cell().BorderBottom(1).BorderColor("#EEE").Padding(6).Text("Tontines").FontSize(9);
                            table.Cell().BorderBottom(1).BorderColor("#EEE").Padding(6).AlignRight().Text(r.NbTontines.ToString()).FontSize(9).Bold();
                            table.Cell().BorderBottom(1).BorderColor("#EEE").Padding(6).Text("Cotisations payées").FontSize(9);
                            table.Cell().BorderBottom(1).BorderColor("#EEE").Padding(6).AlignRight().Text(r.CotisationsPayees.ToString()).FontSize(9).Bold().FontColor("#0F6E56");
                            table.Cell().BorderBottom(1).BorderColor("#EEE").Padding(6).Text("Cotisations en attente").FontSize(9);
                            table.Cell().BorderBottom(1).BorderColor("#EEE").Padding(6).AlignRight().Text(r.CotisationsEnAttente.ToString()).FontSize(9).Bold().FontColor("#F57F17");
                            table.Cell().BorderBottom(1).BorderColor("#EEE").Padding(6).Text("Cotisations en retard").FontSize(9);
                            table.Cell().BorderBottom(1).BorderColor("#EEE").Padding(6).AlignRight().Text(r.CotisationsEnRetard.ToString()).FontSize(9).Bold().FontColor("#C62828");
                            table.Cell().BorderBottom(1).BorderColor("#EEE").Padding(6).Text("Prêts approuvés").FontSize(9);
                            table.Cell().BorderBottom(1).BorderColor("#EEE").Padding(6).AlignRight().Text(r.PretsApprouves.ToString()).FontSize(9).Bold();
                            table.Cell().BorderBottom(1).BorderColor("#EEE").Padding(6).Text("Prêts remboursés").FontSize(9);
                            table.Cell().BorderBottom(1).BorderColor("#EEE").Padding(6).AlignRight().Text(r.PretsRembourses.ToString()).FontSize(9).Bold();
                            table.Cell().Padding(6).Text("Prêts en retard").FontSize(9);
                            table.Cell().Padding(6).AlignRight().Text(r.PretsEnRetard.ToString()).FontSize(9).Bold().FontColor("#C62828");
                        });

                        if (r.ParTontine.Any())
                        {
                            col.Item().Background("#1565C0").Padding(8).Text("COTISATIONS PAR TONTINE").FontColor(Colors.White).Bold().FontSize(10);
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c => { c.RelativeColumn(); c.ConstantColumn(120); });
                                foreach (var kv in r.ParTontine.OrderByDescending(x => x.Value))
                                {
                                    table.Cell().BorderBottom(1).BorderColor("#EEE").Padding(6).Text(kv.Key).FontSize(9);
                                    table.Cell().BorderBottom(1).BorderColor("#EEE").Padding(6).AlignRight().Text(F(kv.Value)).FontSize(9).Bold().FontColor("#1565C0");
                                }
                            });
                        }

                        if (r.TopMembres.Any())
                        {
                            col.Item().Background("#E65100").Padding(8).Text("TOP CONTRIBUTEURS").FontColor(Colors.White).Bold().FontSize(10);
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c => { c.ConstantColumn(30); c.RelativeColumn(); c.ConstantColumn(120); });
                                int rank = 1;
                                foreach (var kv in r.TopMembres.OrderByDescending(x => x.Value))
                                {
                                    int rk = rank++;
                                    table.Cell().BorderBottom(1).BorderColor("#EEE").Padding(5).AlignCenter().Text(rk.ToString()).FontSize(9).Bold().FontColor("#E65100");
                                    table.Cell().BorderBottom(1).BorderColor("#EEE").Padding(5).Text(kv.Key).FontSize(9);
                                    table.Cell().BorderBottom(1).BorderColor("#EEE").Padding(5).AlignRight().Text(F(kv.Value)).FontSize(9).Bold().FontColor("#E65100");
                                }
                            });
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("TontineApp — Document confidentiel — Édité le ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.Span(DateTime.Now.ToString("dd/MM/yyyy à HH:mm")).FontSize(8).FontColor(Colors.Grey.Medium);
                        x.Span("   Page ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                        x.Span(" / ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });
            }).GeneratePdf();
        }

        public byte[] GenererProcesVerbalPdf(ReunionViewModel reunion, List<PresenceViewModel> presences)
        {
            int presents = presences.Count(p => p.StatutPresence == "Présent");
            int excuses  = presences.Count(p => p.StatutPresence == "Excusé");
            int absents  = presences.Count(p => p.StatutPresence == "Absent");
            int total    = presences.Count;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20, Unit.Millimetre);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().BorderBottom(2).BorderColor("#1565C0").PaddingBottom(10).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("TontineApp").FontSize(18).FontColor("#0F6E56").Bold();
                            col.Item().Text("Procès-verbal de réunion").FontSize(11).FontColor(Colors.Grey.Darken1);
                        });
                        row.ConstantItem(140).AlignRight().Column(col =>
                        {
                            col.Item().AlignRight().Text("DOCUMENT OFFICIEL").FontSize(8).Bold().FontColor("#1565C0");
                            col.Item().AlignRight().Text($"Édité le {DateTime.Now:dd/MM/yyyy à HH:mm}").FontSize(8).FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Content().Column(col =>
                    {
                        col.Spacing(12);

                        // Informations de la réunion
                        col.Item().Background("#EEF4FF").Border(1).BorderColor("#BBDEFB").Padding(14).Column(info =>
                        {
                            info.Item().Text("INFORMATIONS DE LA RÉUNION").FontSize(9).Bold().FontColor("#1565C0");
                            info.Item().PaddingTop(8).Table(table =>
                            {
                                table.ColumnsDefinition(c => { c.ConstantColumn(110); c.RelativeColumn(); });
                                table.Cell().Padding(4).Text("Tontine").FontSize(9).FontColor(Colors.Grey.Darken1);
                                table.Cell().Padding(4).Text(reunion.LibelleTontine).Bold().FontSize(9);
                                table.Cell().Padding(4).Text("Cycle").FontSize(9).FontColor(Colors.Grey.Darken1);
                                table.Cell().Padding(4).Text(reunion.NomCycle).Bold().FontSize(9);
                                table.Cell().Padding(4).Text("Date").FontSize(9).FontColor(Colors.Grey.Darken1);
                                table.Cell().Padding(4).Text(reunion.DateReunion.ToString("dddd dd MMMM yyyy à HH:mm")).Bold().FontSize(9);
                                if (!string.IsNullOrEmpty(reunion.Lieu))
                                {
                                    table.Cell().Padding(4).Text("Lieu").FontSize(9).FontColor(Colors.Grey.Darken1);
                                    table.Cell().Padding(4).Text(reunion.Lieu).Bold().FontSize(9);
                                }
                                table.Cell().Padding(4).Text("Objet").FontSize(9).FontColor(Colors.Grey.Darken1);
                                table.Cell().Padding(4).Text(reunion.Objet).Bold().FontSize(9);
                                table.Cell().Padding(4).Text("Statut").FontSize(9).FontColor(Colors.Grey.Darken1);
                                table.Cell().Padding(4).Text(reunion.Statut).Bold().FontSize(9).FontColor(reunion.Statut == "Terminée" ? "#0F6E56" : "#F57C00");
                            });
                        });

                        // Résumé présences
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Padding(3).Background("#E6F4EE").Padding(10).Column(c =>
                            {
                                c.Item().Text("PRÉSENTS").FontSize(8).Bold().FontColor("#0F6E56");
                                c.Item().Text($"{presents}").FontSize(20).Bold().FontColor("#0F6E56");
                                if (total > 0) c.Item().Text($"{100 * presents / total}%").FontSize(8).FontColor("#0F6E56");
                            });
                            row.ConstantItem(6);
                            row.RelativeItem().Padding(3).Background("#FFF8E1").Padding(10).Column(c =>
                            {
                                c.Item().Text("EXCUSÉS").FontSize(8).Bold().FontColor("#F57C00");
                                c.Item().Text($"{excuses}").FontSize(20).Bold().FontColor("#F57C00");
                                if (total > 0) c.Item().Text($"{100 * excuses / total}%").FontSize(8).FontColor("#F57C00");
                            });
                            row.ConstantItem(6);
                            row.RelativeItem().Padding(3).Background("#FFEBEE").Padding(10).Column(c =>
                            {
                                c.Item().Text("ABSENTS").FontSize(8).Bold().FontColor("#C62828");
                                c.Item().Text($"{absents}").FontSize(20).Bold().FontColor("#C62828");
                                if (total > 0) c.Item().Text($"{100 * absents / total}%").FontSize(8).FontColor("#C62828");
                            });
                            row.ConstantItem(6);
                            row.RelativeItem().Padding(3).Background("#F3F3F3").Padding(10).Column(c =>
                            {
                                c.Item().Text("TOTAL").FontSize(8).Bold().FontColor("#555");
                                c.Item().Text($"{total}").FontSize(20).Bold().FontColor("#333");
                            });
                        });

                        // Feuille de présence
                        if (presences.Any())
                        {
                            col.Item().Background("#1565C0").Padding(8).Text("FEUILLE DE PRÉSENCE").FontColor(Colors.White).Bold().FontSize(10);
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.ConstantColumn(25);
                                    c.RelativeColumn();
                                    c.ConstantColumn(80);
                                    c.RelativeColumn(2);
                                });
                                table.Header(h =>
                                {
                                    h.Cell().Background("#E3F2FD").Padding(5).AlignCenter().Text("#").Bold().FontSize(9);
                                    h.Cell().Background("#E3F2FD").Padding(5).Text("Membre").Bold().FontSize(9);
                                    h.Cell().Background("#E3F2FD").Padding(5).AlignCenter().Text("Statut").Bold().FontSize(9);
                                    h.Cell().Background("#E3F2FD").Padding(5).Text("Remarque").Bold().FontSize(9);
                                });
                                int idx = 1;
                                foreach (var p in presences.OrderBy(x => x.NomMembre))
                                {
                                    string bg      = idx % 2 == 0 ? "#FAFAFA" : Colors.White;
                                    string sColor  = p.StatutPresence switch { "Présent" => "#0F6E56", "Excusé" => "#F57C00", _ => "#C62828" };
                                    table.Cell().Background(bg).BorderBottom(1).BorderColor("#EEE").Padding(5).AlignCenter().Text(idx.ToString()).FontSize(9).FontColor(Colors.Grey.Darken1);
                                    table.Cell().Background(bg).BorderBottom(1).BorderColor("#EEE").Padding(5).Text(p.NomMembre).FontSize(9).Bold();
                                    table.Cell().Background(bg).BorderBottom(1).BorderColor("#EEE").Padding(5).AlignCenter().Text(p.StatutPresence).FontSize(9).FontColor(sColor).Bold();
                                    table.Cell().Background(bg).BorderBottom(1).BorderColor("#EEE").Padding(5).Text(p.Remarque ?? "").FontSize(9).FontColor(Colors.Grey.Darken1);
                                    idx++;
                                }
                            });
                        }

                        // Procès-verbal / Notes
                        if (!string.IsNullOrWhiteSpace(reunion.Notes) || !string.IsNullOrWhiteSpace(reunion.ProcesVerbal))
                        {
                            col.Item().Background("#0F6E56").Padding(8).Text("PROCÈS-VERBAL").FontColor(Colors.White).Bold().FontSize(10);
                            col.Item().Border(1).BorderColor("#C8E6C9").Padding(14).Column(pv =>
                            {
                                if (!string.IsNullOrWhiteSpace(reunion.Notes))
                                {
                                    pv.Item().Text("Notes").FontSize(9).Bold().FontColor("#0F6E56");
                                    pv.Item().PaddingTop(4).PaddingBottom(8).Text(reunion.Notes).FontSize(10);
                                }
                                if (!string.IsNullOrWhiteSpace(reunion.ProcesVerbal))
                                {
                                    pv.Item().Text("Délibérations et décisions").FontSize(9).Bold().FontColor("#0F6E56");
                                    pv.Item().PaddingTop(4).Text(reunion.ProcesVerbal).FontSize(10);
                                }
                            });
                        }

                        // Espace signatures
                        col.Item().PaddingTop(20).Row(row =>
                        {
                            row.RelativeItem().Column(sig =>
                            {
                                sig.Item().Text("Le Président").FontSize(9).Bold();
                                sig.Item().PaddingTop(30).BorderBottom(1).BorderColor("#999").Text("");
                                sig.Item().PaddingTop(4).Text("Signature").FontSize(8).FontColor(Colors.Grey.Medium);
                            });
                            row.ConstantItem(40);
                            row.RelativeItem().Column(sig =>
                            {
                                sig.Item().Text("Le Secrétaire").FontSize(9).Bold();
                                sig.Item().PaddingTop(30).BorderBottom(1).BorderColor("#999").Text("");
                                sig.Item().PaddingTop(4).Text("Signature").FontSize(8).FontColor(Colors.Grey.Medium);
                            });
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("TontineApp — Document confidentiel — ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.Span(DateTime.Now.ToString("dd/MM/yyyy à HH:mm")).FontSize(8).FontColor(Colors.Grey.Medium);
                        x.Span("   Page ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                        x.Span("/").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });
            }).GeneratePdf();
        }

        private void ComposeHeader(IContainer c)
        {
            c.BorderBottom(1).BorderColor("#0F6E56").PaddingBottom(10).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("TontineApp").FontSize(18).FontColor("#0F6E56").Bold();
                    col.Item().Text("Relevé de compte membre").FontSize(11).FontColor(Colors.Grey.Darken1);
                });
                row.ConstantItem(120).AlignRight().Column(col =>
                {
                    col.Item().AlignRight().Text("DOCUMENT OFFICIEL").FontSize(8).Bold().FontColor("#0F6E56");
                    col.Item().AlignRight().Text("Gestion des associations").FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });
        }

        private void ComposeFicheMembre(IContainer c, MembreViewModel m)
        {
            c.Background("#F8FDF9").Border(1).BorderColor("#C8E6C9").Padding(12).Column(col =>
            {
                col.Item().Text("INFORMATIONS DU MEMBRE").FontSize(9).Bold().FontColor("#0F6E56");
                col.Item().PaddingTop(6).Row(row =>
                {
                    row.RelativeItem().Column(info =>
                    {
                        info.Item().Text($"{m.Nom} {m.Prenom}").FontSize(14).Bold();
                        if (!string.IsNullOrEmpty(m.Telephone))
                            info.Item().Text($"Tél : {m.Telephone}").FontColor(Colors.Grey.Darken1);
                        if (!string.IsNullOrEmpty(m.Email))
                            info.Item().Text($"Email : {m.Email}").FontColor(Colors.Grey.Darken1);
                    });
                    row.RelativeItem().Column(info =>
                    {
                        if (!string.IsNullOrEmpty(m.Ville) || !string.IsNullOrEmpty(m.Pays))
                            info.Item().Text($"Localisation : {m.Ville} {m.Pays}").FontColor(Colors.Grey.Darken1);
                        if (!string.IsNullOrEmpty(m.Profession))
                            info.Item().Text($"Profession : {m.Profession}").FontColor(Colors.Grey.Darken1);
                        info.Item().Text($"Inscrit le : {m.DateInscription:dd/MM/yyyy}").FontColor(Colors.Grey.Darken1);
                    });
                });
            });
        }

        private void ComposeResume(IContainer c, ReleveMembreViewModel r)
        {
            c.Row(row =>
            {
                row.RelativeItem().Padding(2).Background("#E6F4EE").Padding(10).Column(col =>
                {
                    col.Item().Text("COTISATIONS").FontSize(8).Bold().FontColor("#0F6E56");
                    col.Item().Text($"{r.TotalCotisations:N0} FCFA").FontSize(14).Bold().FontColor("#0F6E56");
                    col.Item().Text($"{r.Cotisations.Count} opération(s)").FontSize(8).FontColor(Colors.Grey.Darken1);
                });
                row.ConstantItem(8);
                row.RelativeItem().Padding(2).Background("#F3E5F5").Padding(10).Column(col =>
                {
                    col.Item().Text("VERSEMENTS REÇUS").FontSize(8).Bold().FontColor("#6A1B9A");
                    col.Item().Text($"{r.TotalVersements:N0} FCFA").FontSize(14).Bold().FontColor("#6A1B9A");
                    col.Item().Text($"{r.Versements.Count} versement(s)").FontSize(8).FontColor(Colors.Grey.Darken1);
                });
                row.ConstantItem(8);
                row.RelativeItem().Padding(2).Background("#FFEBEE").Padding(10).Column(col =>
                {
                    col.Item().Text("PRÊTS ACCORDÉS").FontSize(8).Bold().FontColor("#C62828");
                    col.Item().Text($"{r.TotalPrets:N0} FCFA").FontSize(14).Bold().FontColor("#C62828");
                    col.Item().Text($"{r.Prets.Count} prêt(s)").FontSize(8).FontColor(Colors.Grey.Darken1);
                });
            });
        }

        private void ComposeCotisations(IContainer c, List<CotisationReleveDto> items, decimal total)
        {
            c.Column(col =>
            {
                col.Item().Background("#0F6E56").Padding(8)
                    .Text("COTISATIONS").FontColor(Colors.White).Bold().FontSize(10);

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(70); // Date
                        cols.RelativeColumn(2);  // Tontine
                        cols.RelativeColumn(2);  // Cycle
                        cols.ConstantColumn(90); // Montant
                        cols.ConstantColumn(65); // Statut
                    });

                    // En-tête
                    table.Header(h =>
                    {
                        h.Cell().Background("#F0FDF4").Padding(5).Text("Date").Bold().FontSize(9);
                        h.Cell().Background("#F0FDF4").Padding(5).Text("Tontine").Bold().FontSize(9);
                        h.Cell().Background("#F0FDF4").Padding(5).Text("Cycle").Bold().FontSize(9);
                        h.Cell().Background("#F0FDF4").Padding(5).AlignRight().Text("Montant").Bold().FontSize(9);
                        h.Cell().Background("#F0FDF4").Padding(5).Text("Statut").Bold().FontSize(9);
                    });

                    foreach (var (item, i) in items.Select((x, i) => (x, i)))
                    {
                        string bg = i % 2 == 0 ? Colors.White : "#FAFAFA";
                        table.Cell().Background(bg).BorderBottom(1).BorderColor("#EEE").Padding(5).Text(item.DateCotisation.ToString("dd/MM/yy")).FontSize(9).FontColor(Colors.Grey.Darken1);
                        table.Cell().Background(bg).BorderBottom(1).BorderColor("#EEE").Padding(5).Text(item.LibelleTontine).FontSize(9);
                        table.Cell().Background(bg).BorderBottom(1).BorderColor("#EEE").Padding(5).Text(item.NomCycle).FontSize(9).FontColor(Colors.Grey.Darken1);
                        table.Cell().Background(bg).BorderBottom(1).BorderColor("#EEE").Padding(5).AlignRight().Text($"{item.Montant:N0} FCFA").FontSize(9).Bold().FontColor("#0F6E56");
                        table.Cell().Background(bg).BorderBottom(1).BorderColor("#EEE").Padding(5).Text(item.Statut).FontSize(9);
                    }

                    // Total
                    table.Cell().ColumnSpan(3).Background("#E6F4EE").Padding(5).AlignRight().Text("TOTAL").Bold().FontSize(9);
                    table.Cell().Background("#E6F4EE").Padding(5).AlignRight().Text($"{total:N0} FCFA").Bold().FontSize(9).FontColor("#0F6E56");
                    table.Cell().Background("#E6F4EE").Padding(5).Text("");
                });
            });
        }

        private void ComposeVersements(IContainer c, List<VersementReleveDto> items, decimal total)
        {
            c.Column(col =>
            {
                col.Item().Background("#6A1B9A").Padding(8)
                    .Text("VERSEMENTS REÇUS").FontColor(Colors.White).Bold().FontSize(10);

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(70);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(2);
                        cols.ConstantColumn(90);
                    });

                    table.Header(h =>
                    {
                        h.Cell().Background("#F9F0FF").Padding(5).Text("Date").Bold().FontSize(9);
                        h.Cell().Background("#F9F0FF").Padding(5).Text("Tontine").Bold().FontSize(9);
                        h.Cell().Background("#F9F0FF").Padding(5).Text("Cycle").Bold().FontSize(9);
                        h.Cell().Background("#F9F0FF").Padding(5).AlignRight().Text("Montant").Bold().FontSize(9);
                    });

                    foreach (var (item, i) in items.Select((x, i) => (x, i)))
                    {
                        string bg = i % 2 == 0 ? Colors.White : "#FAFAFA";
                        table.Cell().Background(bg).BorderBottom(1).BorderColor("#EEE").Padding(5).Text(item.DateVersement.ToString("dd/MM/yy")).FontSize(9).FontColor(Colors.Grey.Darken1);
                        table.Cell().Background(bg).BorderBottom(1).BorderColor("#EEE").Padding(5).Text(item.LibelleTontine).FontSize(9);
                        table.Cell().Background(bg).BorderBottom(1).BorderColor("#EEE").Padding(5).Text(item.NomCycle).FontSize(9).FontColor(Colors.Grey.Darken1);
                        table.Cell().Background(bg).BorderBottom(1).BorderColor("#EEE").Padding(5).AlignRight().Text($"{item.Montant:N0} FCFA").FontSize(9).Bold().FontColor("#6A1B9A");
                    }

                    table.Cell().ColumnSpan(2).Background("#F3E5F5").Padding(5).AlignRight().Text("TOTAL").Bold().FontSize(9);
                    table.Cell().Background("#F3E5F5").Padding(5).AlignRight().Text($"{total:N0} FCFA").Bold().FontSize(9).FontColor("#6A1B9A");
                    table.Cell().Background("#F3E5F5").Padding(5).Text("");
                });
            });
        }

        private void ComposePrets(IContainer c, List<PretViewModel> items, decimal total)
        {
            c.Column(col =>
            {
                col.Item().Background("#C62828").Padding(8)
                    .Text("PRÊTS").FontColor(Colors.White).Bold().FontSize(10);

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(70);
                        cols.ConstantColumn(90);
                        cols.ConstantColumn(40);
                        cols.ConstantColumn(65);
                        cols.RelativeColumn();
                        cols.ConstantColumn(70);
                    });

                    table.Header(h =>
                    {
                        h.Cell().Background("#FFF5F5").Padding(5).Text("Date").Bold().FontSize(9);
                        h.Cell().Background("#FFF5F5").Padding(5).AlignRight().Text("Montant").Bold().FontSize(9);
                        h.Cell().Background("#FFF5F5").Padding(5).AlignCenter().Text("Taux").Bold().FontSize(9);
                        h.Cell().Background("#FFF5F5").Padding(5).Text("Statut").Bold().FontSize(9);
                        h.Cell().Background("#FFF5F5").Padding(5).Text("Description").Bold().FontSize(9);
                        h.Cell().Background("#FFF5F5").Padding(5).Text("Échéance").Bold().FontSize(9);
                    });

                    foreach (var (p, i) in items.Select((x, i) => (x, i)))
                    {
                        string bg = i % 2 == 0 ? Colors.White : "#FAFAFA";
                        string statutColor = p.Statut switch { "Remboursé" => "#0F6E56", "En retard" => "#C62828", "Approuvé" => "#1565C0", _ => "#F57F17" };
                        table.Cell().Background(bg).BorderBottom(1).BorderColor("#EEE").Padding(5).Text(p.DatePret.ToString("dd/MM/yy")).FontSize(9).FontColor(Colors.Grey.Darken1);
                        table.Cell().Background(bg).BorderBottom(1).BorderColor("#EEE").Padding(5).AlignRight().Text($"{p.Montant:N0} FCFA").FontSize(9).Bold().FontColor("#C62828");
                        table.Cell().Background(bg).BorderBottom(1).BorderColor("#EEE").Padding(5).AlignCenter().Text($"{p.Taux}%").FontSize(9);
                        table.Cell().Background(bg).BorderBottom(1).BorderColor("#EEE").Padding(5).Text(p.Statut).FontSize(9).FontColor(statutColor).Bold();
                        table.Cell().Background(bg).BorderBottom(1).BorderColor("#EEE").Padding(5).Text(p.Description ?? "—").FontSize(9).FontColor(Colors.Grey.Darken1);
                        table.Cell().Background(bg).BorderBottom(1).BorderColor("#EEE").Padding(5).Text(p.DateRemboursement == DateTime.MinValue ? "—" : p.DateRemboursement.ToString("dd/MM/yy")).FontSize(9);
                    }

                    table.Cell().ColumnSpan(4).Background("#FFEBEE").Padding(5).AlignRight().Text("TOTAL EMPRUNTÉ").Bold().FontSize(9);
                    table.Cell().Background("#FFEBEE").Padding(5).Text($"{total:N0} FCFA").Bold().FontSize(9).FontColor("#C62828");
                    table.Cell().Background("#FFEBEE").Padding(5).Text("");
                });
            });
        }
    }
}
