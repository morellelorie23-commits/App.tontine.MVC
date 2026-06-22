/**
 * saisie-seance.js — Logique SPA de la Saisie Séance Tontine
 * Architecture : Vanilla JS + Bootstrap 5 Collapse API
 */

'use strict';

// ══════════════════════════════════════════════════════════════════════
// ÉTAT GLOBAL
// ══════════════════════════════════════════════════════════════════════
const Seance = {
    idCycle:          0,       // Injecté depuis la vue Razor
    idTontine:        0,
    idReunion:        0,
    soldeCaisse:      0,
    membres:          [],      // [{ idMembre, nomPrenom, mtAttendu }]
    tontineConfirmee: false,
    seanceChargee:    false,
    etapeActuelle:    1        // 1 | 2 | 3
};

// ══════════════════════════════════════════════════════════════════════
// FORMAT MONNAIE
// ══════════════════════════════════════════════════════════════════════
function fmt(n) {
    return new Intl.NumberFormat('fr-FR').format(Math.round(n || 0)) + ' F';
}

// ══════════════════════════════════════════════════════════════════════
// BARRE DE PROGRESSION
// ══════════════════════════════════════════════════════════════════════
function setEtape(n) {
    Seance.etapeActuelle = n;
    document.querySelectorAll('.ss-step').forEach((el, i) => {
        el.classList.toggle('active',   i + 1 === n);
        el.classList.toggle('done',     i + 1 < n);
        el.classList.toggle('pending',  i + 1 > n);
    });
}

// ══════════════════════════════════════════════════════════════════════
// ACCORDIONS — ouvrir / fermer via Bootstrap API
// ══════════════════════════════════════════════════════════════════════
function ouvrirAccordion(num) {
    ['1','2','3'].forEach(n => {
        const el = document.getElementById('collapse' + n);
        if (!el) return;
        const bsCol = bootstrap.Collapse.getOrCreateInstance(el, { toggle: false });
        if (n === String(num)) bsCol.show(); else bsCol.hide();
    });
    setEtape(num);
}

// ══════════════════════════════════════════════════════════════════════
// ACCORDÉON 1 — Chargement tontines / réunions / confirmation
// ══════════════════════════════════════════════════════════════════════

/** Quand l'utilisateur clique "Confirmer Tontine" */
async function confirmerTontine() {
    const sel = document.getElementById('selTontine');
    Seance.idTontine = parseInt(sel.value);
    if (!Seance.idTontine) { showToast('Veuillez sélectionner une tontine.', 'warning'); return; }

    const btn = document.getElementById('btnConfirmerTontine');
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Chargement...';
    btn.disabled = true;

    try {
        // Charger réunions
        const res = await fetch(`/SaisieSeance/GetReunions?idTontine=${Seance.idTontine}`);
        const reunions = await res.json();

        const selR = document.getElementById('selReunion');
        selR.innerHTML = '<option value="">— Sélectionner —</option>';
        reunions.forEach(r => {
            const label = r.dateReunion.substring(0, 10) + (r.objet ? ' — ' + r.objet : '');
            selR.innerHTML += `<option value="${r.idReunion}">${label}</option>`;
        });

        // Charger membres immédiatement pour pré-remplir la table
        const resM = await fetch(`/SaisieSeance/GetMembres?idTontine=${Seance.idTontine}`);
        Seance.membres = await resM.json();

        // Marquer tontine confirmée
        Seance.tontineConfirmee = true;
        btn.innerHTML = '<i class="fa-solid fa-check me-2"></i>Confirmé !';
        btn.classList.remove('btn-outline-success');
        btn.classList.add('btn-success');

        // Afficher la section réunion
        document.getElementById('sectionReunion').style.display = '';
        fadeIn(document.getElementById('sectionReunion'));

        showToast('Tontine confirmée. Sélectionnez une réunion.', 'success');
    } catch (e) {
        showToast('Erreur lors du chargement.', 'danger');
    } finally {
        btn.disabled = false;
    }
}

/** Quand l'utilisateur sélectionne une réunion */
async function chargerSeance() {
    Seance.idReunion = parseInt(document.getElementById('selReunion').value);
    if (!Seance.idReunion) return;

    const btn = document.getElementById('btnChargerSeance');
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>';
    btn.disabled = true;

    try {
        const res = await fetch(`/SaisieSeance/GetData?idTontine=${Seance.idTontine}&idReunion=${Seance.idReunion}`);
        const data = await res.json();

        Seance.soldeCaisse = data.soldeCaisse;
        Seance.membres = data.lignes;

        // Mettre à jour le solde caisse
        document.getElementById('soldeCaisse').textContent = fmt(data.soldeCaisse);

        // Construire le tableau membres
        construireTableauMembres(data.lignes);
        mettreAJourHistorique(data.dejabeneficiaires);

        Seance.seanceChargee = true;
        btn.innerHTML = '<i class="fa-solid fa-check me-2"></i>Chargé';
        btn.classList.replace('btn-outline-primary', 'btn-primary');

        // Passer à l'accordion 2 avec délai animé
        setTimeout(() => ouvrirAccordion(2), 350);
        showToast('Membres chargés ! Saisissez les cotisations.', 'success');
    } catch (e) {
        showToast('Impossible de charger les données de la séance.', 'danger');
    } finally {
        btn.disabled = false;
    }
}

// ══════════════════════════════════════════════════════════════════════
// ACCORDÉON 2 — Tableau membres
// ══════════════════════════════════════════════════════════════════════

function construireTableauMembres(lignes) {
    const tbody = document.getElementById('membresTableBody');
    tbody.innerHTML = '';

    lignes.forEach((l, idx) => {
        const reste = l.mtAttendu - l.mtCotise;
        const badgeClass = reste > 0 ? 'bg-danger' : 'bg-success';

        const tr = document.createElement('tr');
        tr.dataset.idMembre  = l.idMembre ?? l.id_membre ?? l.idmembre ?? 0;
        tr.dataset.mtAttendu = l.mtAttendu ?? l.mt_attendu ?? 0;

        tr.innerHTML = `
            <td class="text-center text-muted fw-bold" style="width:44px;">${idx + 1}</td>
            <td>
                <div class="d-flex align-items-center gap-2">
                    <div class="membre-avatar">${initiales(l.nomPrenom ?? l.nom_prenom ?? '')}</div>
                    <span class="fw-600">${esc(l.nomPrenom ?? l.nom_prenom ?? '')}</span>
                </div>
                <input type="hidden" class="h-id"        value="${l.idMembre ?? 0}">
                <input type="hidden" class="h-mtattendu" value="${l.mtAttendu ?? 0}">
            </td>
            <td class="text-end">
                <span class="badge bg-light text-muted border px-3 py-2" style="font-size:13px;">
                    ${fmt(l.mtAttendu)}
                </span>
            </td>
            <td style="width:150px;">
                <div class="input-group input-group-sm">
                    <input type="number" class="form-control inp-paye ss-input-paye" min="0" step="500"
                           value="${l.mtCotise > 0 ? l.mtCotise : ''}" placeholder="0"
                           style="border-radius:10px!important;">
                </div>
            </td>
            <td style="width:110px;">
                <input type="number" class="form-control form-control-sm inp-penalite ss-input-penalite" min="0" step="100"
                       value="${l.penalite > 0 ? l.penalite : ''}" placeholder="0"
                       style="border-radius:10px!important; border-color:#fd7e14;">
            </td>
            <td style="width:110px;">
                <input type="number" class="form-control form-control-sm inp-enchere ss-input-enchere" min="0" step="100"
                       value="${l.mtEnchere > 0 ? l.mtEnchere : ''}" placeholder="0"
                       style="border-radius:10px!important; border-color:#0d6efd;">
            </td>
            <td style="width:180px;">
                <select class="form-select form-select-sm inp-mode"
                        style="border-radius:10px!important; border-color:#6f42c1; font-size:12px;">
                    <option value="Cash"         ${l.modePaiement === 'Cash'         ? 'selected' : ''}>💵 Cash</option>
                    <option value="MTN"          ${l.modePaiement === 'MTN'          ? 'selected' : ''}>📱 MTN</option>
                    <option value="Orange Money" ${l.modePaiement === 'Orange Money' ? 'selected' : ''}>🟠 Orange Money</option>
                </select>
                <input type="text" class="form-control form-control-sm inp-tel mt-1"
                       placeholder="Téléphone (ex: 237...)"
                       style="display:none; border-radius:10px!important; border-color:#6f42c1; font-size:12px;">
            </td>
            <td class="text-center" style="width:120px;">
                <span class="badge ${badgeClass} reste-badge px-3 py-2">${fmt(reste)}</span>
            </td>`;
        tbody.appendChild(tr);
    });

    // Afficher le tableau, cacher le message vide
    document.getElementById('tableVideMsg').style.display   = 'none';
    document.getElementById('tableWrapper').style.display   = '';
    document.getElementById('btnSuivantWrapper').style.display = '';

    attacherEvenementsTable();
    recalcTotaux();
}

function attacherEvenementsTable() {
    document.querySelectorAll('.inp-paye, .inp-penalite').forEach(inp => {
        inp.addEventListener('input', function () {
            recalcLigne(this.closest('tr'));
            recalcTotaux();
            detecterGagnant();
        });
    });
    document.querySelectorAll('.inp-enchere').forEach(inp => {
        inp.addEventListener('input', function () {
            detecterGagnant();
        });
    });
    document.querySelectorAll('.inp-mode').forEach(sel => {
        sel.addEventListener('change', function () {
            const tel = this.closest('td').querySelector('.inp-tel');
            tel.style.display = (this.value === 'MTN' || this.value === 'Orange Money') ? '' : 'none';
        });
    });
}

function recalcLigne(tr) {
    const mtAttendu = parseFloat(tr.dataset.mtAttendu) || 0;
    const mtPaye    = parseFloat(tr.querySelector('.inp-paye').value) || 0;
    const penalite  = parseFloat(tr.querySelector('.inp-penalite').value) || 0;
    const reste     = mtAttendu - mtPaye + penalite;

    const badge = tr.querySelector('.reste-badge');
    badge.textContent = fmt(Math.max(0, reste));
    if (reste <= 0) {
        badge.className = 'badge bg-success reste-badge px-3 py-2';
    } else {
        badge.className = 'badge bg-danger reste-badge px-3 py-2';
    }

    // Couleur input pénalité si > 0
    const inpP = tr.querySelector('.inp-penalite');
    inpP.style.borderColor = parseFloat(inpP.value) > 0 ? '#fd7e14' : '#dee2e6';
}

function recalcTotaux() {
    let totalAttendu = 0, totalPaye = 0;

    document.querySelectorAll('#membresTableBody tr').forEach(tr => {
        totalAttendu += parseFloat(tr.dataset.mtAttendu) || 0;
        totalPaye    += parseFloat(tr.querySelector('.inp-paye')?.value) || 0;
    });

    document.getElementById('badgeTotalPaye').textContent = fmt(totalPaye);
    document.getElementById('totalCollecte').value  = totalPaye;
    document.getElementById('totalCollecteDisplay').textContent = fmt(totalPaye);

    // Activer bouton valider si au moins un paiement saisi
    document.getElementById('btnValider').disabled = totalPaye <= 0;
}

/** Détecte le membre avec l'enchère la plus haute → surbrillance + pré-remplissage Accord 3 */
function detecterGagnant() {
    let maxEnchere = 0, gagnantTr = null, gagnantNom = '', gagnantId = 0;

    document.querySelectorAll('#membresTableBody tr').forEach(tr => {
        const enc = parseFloat(tr.querySelector('.inp-enchere')?.value) || 0;
        if (enc > maxEnchere) {
            maxEnchere = enc;
            gagnantTr  = tr;
            gagnantNom = tr.querySelector('.fw-600')?.textContent?.trim() || '';
            gagnantId  = parseInt(tr.dataset.idMembre) || 0;
        }
        // Retirer surbrillance de toutes les lignes
        tr.classList.remove('table-warning', 'gagnant-row');
    });

    if (gagnantTr && maxEnchere > 0) {
        gagnantTr.classList.add('table-warning', 'gagnant-row');

        // Pré-remplir Accordion 3
        document.getElementById('gagnantNom').textContent = gagnantNom;
        document.getElementById('gagnantEnchere').textContent = fmt(maxEnchere);
        document.getElementById('selBeneficiaire').value = gagnantId;
        mettreAJourMontantBeneficie();

        // Badge gagnant dans l'en-tête accordion 3
        document.getElementById('badgeGagnant').style.display = '';
        document.getElementById('badgeGagnant').textContent = '🏆 Gagnant : ' + gagnantNom;
    } else {
        document.getElementById('badgeGagnant').style.display = 'none';
    }
}

function passerAResultat() {
    // Vérification : au moins un membre a saisi un paiement
    let totalPaye = 0;
    document.querySelectorAll('.inp-paye').forEach(i => totalPaye += parseFloat(i.value) || 0);
    if (totalPaye <= 0) {
        showToast('Veuillez saisir au moins un paiement avant de continuer.', 'warning');
        return;
    }
    mettreAJourMontantBeneficie();
    ouvrirAccordion(3);
    setTimeout(() => document.getElementById('collapse3')?.scrollIntoView({ behavior: 'smooth', block: 'start' }), 350);
}

// ══════════════════════════════════════════════════════════════════════
// ACCORDÉON 3 — Résultat
// ══════════════════════════════════════════════════════════════════════

function mettreAJourMontantBeneficie() {
    const idBenef = parseInt(document.getElementById('selBeneficiaire').value) || 0;
    let totalPaye = 0, enchereBenef = 0;

    document.querySelectorAll('#membresTableBody tr').forEach(tr => {
        totalPaye += parseFloat(tr.querySelector('.inp-paye')?.value) || 0;
        if (parseInt(tr.dataset.idMembre) === idBenef) {
            enchereBenef = parseFloat(tr.querySelector('.inp-enchere')?.value) || 0;
        }
    });

    const montantVerse = totalPaye - enchereBenef;
    document.getElementById('montantVerse').textContent = fmt(montantVerse);
    document.getElementById('montantVerseVal').value = montantVerse;
    document.getElementById('totalCollecteDisplay').textContent = fmt(totalPaye);
}

function mettreAJourHistorique(dejabeneficiaires) {
    const box = document.getElementById('historiqueBox');
    if (!dejabeneficiaires || dejabeneficiaires.length === 0) {
        box.textContent = 'Aucun bénéficiaire pour ce cycle.';
        return;
    }
    box.textContent = dejabeneficiaires
        .map(d => `• ${d.nomPrenom} — ${d.dateVersement} — ${fmt(d.montantNet)}`)
        .join('\n');
}

// ══════════════════════════════════════════════════════════════════════
// VALIDATION FINALE
// ══════════════════════════════════════════════════════════════════════

async function validerSeance() {
    if (!Seance.seanceChargee) {
        showToast('Veuillez charger une séance avant de valider.', 'warning');
        return;
    }

    const idBenef = parseInt(document.getElementById('selBeneficiaire').value) || null;
    const lignes  = [];

    document.querySelectorAll('#membresTableBody tr').forEach(tr => {
        const idMembre     = parseInt(tr.dataset.idMembre) || 0;
        const mtPaye       = parseFloat(tr.querySelector('.inp-paye')?.value) || 0;
        const penalite     = parseFloat(tr.querySelector('.inp-penalite')?.value) || 0;
        const enchere      = parseFloat(tr.querySelector('.inp-enchere')?.value) || 0;
        const modePaiement = tr.querySelector('.inp-mode')?.value || 'Cash';
        const telephone    = tr.querySelector('.inp-tel')?.value?.trim() || '';

        if (idMembre > 0) {
            lignes.push({
                idMembre,
                mtAttendu:        parseFloat(tr.dataset.mtAttendu) || 0,
                mtCotise:         mtPaye,
                penalite,
                mtEnchere:        enchere,
                isGagnantEnchere: idBenef !== null && idMembre === idBenef && enchere > 0,
                modePaiement,
                telephone
            });
        }
    });

    const payload = {
        idTontine:        Seance.idTontine,
        idReunion:        Seance.idReunion,
        idCycle:          Seance.idCycle,
        idBeneficiaire:   idBenef,
        montantBeneficie: parseFloat(document.getElementById('montantVerseVal').value) || 0,
        retourCaisse:     parseFloat(document.getElementById('retourCaisse').value) || 0,
        lignes
    };

    const btn = document.getElementById('btnValider');
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Enregistrement...';

    try {
        const res = await fetch('/SaisieSeance/Enregistrer', {
            method:  'POST',
            headers: { 'Content-Type': 'application/json' },
            body:    JSON.stringify(payload)
        });
        const data = await res.json();

        if (res.ok) {
            afficherSucces(data.message || 'Séance enregistrée avec succès !');
            // Animation de succès dans Accordion 3
            document.getElementById('carteSucces').style.display = '';
            fadeIn(document.getElementById('carteSucces'));
            document.getElementById('formResultat').style.display = 'none';
        } else {
            showToast(data.message || 'Erreur lors de l\'enregistrement.', 'danger');
        }
    } catch (e) {
        showToast('Erreur réseau : ' + e.message, 'danger');
    } finally {
        btn.disabled = false;
        btn.innerHTML = '<i class="fa-solid fa-check-double me-2"></i>VALIDER & METTRE À JOUR CAISSE';
    }
}

// ══════════════════════════════════════════════════════════════════════
// UTILITAIRES
// ══════════════════════════════════════════════════════════════════════

/** Toast Bootstrap 5 dynamique */
function showToast(message, type = 'info') {
    const colors = { success:'#198754', danger:'#dc3545', warning:'#fd7e14', info:'#0d6efd' };
    const id = 'toast_' + Date.now();
    const html = `
        <div id="${id}" class="toast align-items-center text-white border-0" role="alert"
             style="background:${colors[type]||colors.info}; min-width:280px;">
            <div class="d-flex">
                <div class="toast-body fw-500">${esc(message)}</div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>`;
    document.getElementById('toastContainer').insertAdjacentHTML('beforeend', html);
    const toastEl = document.getElementById(id);
    new bootstrap.Toast(toastEl, { delay: 4000 }).show();
    toastEl.addEventListener('hidden.bs.toast', () => toastEl.remove());
}

function afficherSucces(msg) {
    const el = document.getElementById('alerteSucces');
    document.getElementById('alerteSuccesMsg').textContent = msg;
    el.style.display = '';
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

function fadeIn(el) {
    el.style.opacity = '0';
    el.style.transition = 'opacity 0.4s ease';
    requestAnimationFrame(() => requestAnimationFrame(() => el.style.opacity = '1'));
}

function initiales(nom) {
    return nom.split(' ').slice(0, 2).map(w => w[0] || '').join('').toUpperCase();
}

function esc(s) {
    const d = document.createElement('div');
    d.appendChild(document.createTextNode(s || ''));
    return d.innerHTML;
}

// ══════════════════════════════════════════════════════════════════════
// INIT
// ══════════════════════════════════════════════════════════════════════
document.addEventListener('DOMContentLoaded', () => {
    // Récupérer idCycle depuis la meta tag
    const metaCycle = document.querySelector('meta[name="cycle-id"]');
    if (metaCycle) Seance.idCycle = parseInt(metaCycle.content) || 0;

    // Ouvrir accordion 1 par défaut
    ouvrirAccordion(1);

    // Cacher la section réunion au départ
    const secR = document.getElementById('sectionReunion');
    if (secR) secR.style.display = 'none';

    // Bénéficiaire change → recalc montant versé
    const selB = document.getElementById('selBeneficiaire');
    if (selB) selB.addEventListener('change', mettreAJourMontantBeneficie);

    // Retour en caisse change → rien (juste valeur libre)
    console.info('[SaisieSeance] Initialisé. idCycle =', Seance.idCycle);
});
