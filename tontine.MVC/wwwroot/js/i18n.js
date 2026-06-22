// i18n - Support FR/EN avec scan automatique du DOM
const LANGUAGE_KEY = 'tontine-language';
const DEFAULT_LANGUAGE = 'fr';

let currentLanguage = DEFAULT_LANGUAGE;
let translations = {};

// ─── Dictionnaire de textes UI (FR → EN) ─────────────────────────────────────
const AUTO_TRANSLATE = {
  // Titres de pages
  "Gestion des membres": "Member Management",
  "Gestion des cycles": "Cycle Management",
  "Gestion des tontines": "Tontine Management",
  "Gestion des postes": "Position Management",
  "Gestion des pénalités": "Penalty Management",
  "Gestion des Prêts": "Loan Management",
  "Gestion des prêts": "Loan Management",
  "Attribution de rôles": "Role Assignments",
  "Liaison Cycles - Tontines": "Cycles - Tontines Link",
  "Liaison Pénalités - Cycles": "Penalties - Cycles Link",
  "Participation Membres": "Member Participation",
  "Gestion des comptes": "Account Management",
  "Gestion des cotisations": "Contribution Management",
  "Gestion des versements": "Disbursement Management",
  "Tableau de bord": "Dashboard",
  "Statistiques": "Statistics",
  "Journal d'activité": "Activity Log",

  // Sous-titres formulaires
  "Nouveau membre": "New Member",
  "Nouvelle tontine": "New Tontine",
  "Nouveau cycle": "New Cycle",
  "Nouveau poste": "New Position",
  "Nouvelle pénalité": "New Penalty",
  "Nouveau prêt": "New Loan",
  "Nouveau versement": "New Disbursement",
  "Nouvelle cotisation": "New Contribution",
  "Nouveau compte utilisateur": "New User Account",
  "Nouvelle liaison": "New Link",
  "Nouvelle participation": "New Participation",
  "Nouvelle attribution de rôle": "New Role Assignment",
  "Modifier le membre": "Edit Member",
  "Modifier la tontine": "Edit Tontine",
  "Modifier le cycle": "Edit Cycle",
  "Modifier le poste": "Edit Position",
  "Modifier la pénalité": "Edit Penalty",
  "Modifier le prêt": "Edit Loan",
  "Modifier le versement": "Edit Disbursement",
  "Modifier la cotisation": "Edit Contribution",
  "Modifier le compte": "Edit Account",
  "Modifier la liaison": "Edit Link",
  "Modifier la participation": "Edit Participation",
  "Modifier l'attribution de rôle": "Edit Role Assignment",
  "Confirmer la suppression": "Confirm Deletion",
  "Supprimer le membre": "Delete Member",
  "Supprimer la tontine": "Delete Tontine",
  "Supprimer le cycle": "Delete Cycle",
  "Supprimer le poste": "Delete Position",
  "Supprimer la pénalité": "Delete Penalty",
  "Supprimer le prêt": "Delete Loan",
  "Supprimer le versement": "Delete Disbursement",
  "Supprimer la cotisation": "Delete Contribution",
  "Supprimer le compte": "Delete Account",
  "Supprimer la liaison": "Delete Link",

  // En-têtes de colonnes
  "Actions": "Actions",
  "Statut": "Status",
  "Montant": "Amount",
  "Membre": "Member",
  "Tontine": "Tontine",
  "Cycle": "Cycle",
  "Poste": "Position",
  "Pénalité": "Penalty",
  "Nom": "Last Name",
  "Prénom": "First Name",
  "Email": "Email",
  "Téléphone": "Phone",
  "Ville": "City",
  "Inscription": "Registration",
  "Photo": "Photo",
  "Description": "Description",
  "Libellé": "Label",
  "Début": "Start",
  "Fin": "End",
  "Fréquence": "Frequency",
  "Date": "Date",
  "Date prêt": "Loan Date",
  "Échéance": "Due Date",
  "Taux": "Rate",
  "Taux Avant": "Rate Before",
  "Taux Après": "Rate After",
  "Intérêts": "Interest",
  "Commentaire": "Comment",

  // Boutons
  "Modifier": "Edit",
  "Supprimer": "Delete",
  "Enregistrer": "Save",
  "Annuler": "Cancel",
  "Démarrer": "Start",
  "Clôturer": "Close",
  "Approuver": "Approve",
  "Rembourser": "Repay",
  "Relevé": "Statement",
  "Imprimer": "Print",
  "Retour": "Back",
  "Fermer": "Close",
  "Confirmer": "Confirm",
  "Confirmer la suppression": "Confirm Deletion",

  // Boutons "+ Nouveau X"
  "+ Nouveau membre": "+ New Member",
  "+ Nouveau cycle": "+ New Cycle",
  "+ Nouveau poste": "+ New Position",
  "+ Nouveau prêt": "+ New Loan",
  "+ Nouveau versement": "+ New Disbursement",
  "+ Nouveau compte": "+ New Account",
  "+ Nouvelle tontine": "+ New Tontine",
  "+ Nouvelle pénalité": "+ New Penalty",
  "+ Nouvelle cotisation": "+ New Contribution",
  "+ Nouvelle liaison": "+ New Link",

  // Labels de formulaires
  "Nom du membre *": "Member Name *",
  "Prénom *": "First Name *",
  "Nom *": "Last Name *",
  "Email *": "Email *",
  "Téléphone": "Phone",
  "Ville": "City",
  "Date d'inscription *": "Registration Date *",
  "Nom du cycle *": "Cycle Name *",
  "Date de début": "Start Date",
  "Date de fin": "End Date",
  "Nom de la tontine *": "Tontine Name *",
  "Montant de la cotisation *": "Contribution Amount *",
  "Libellé *": "Label *",
  "Taux d'intérêt (%)": "Interest Rate (%)",
  "Montant *": "Amount *",
  "Date *": "Date *",
  "Statut *": "Status *",

  // Statuts
  "En attente": "Pending",
  "Actif": "Active",
  "Terminé": "Completed",
  "En retard": "Overdue",
  "Approuvé": "Approved",
  "Remboursé": "Repaid",
  "Suspendu": "Suspended",
  "Inactif": "Inactive",
  "Bientôt": "Soon",

  // Messages vides
  "Aucun membre enregistré": "No members registered",
  "Aucune tontine enregistrée": "No tontines registered",
  "Aucun cycle enregistré": "No cycles registered",
  "Aucun poste enregistré": "No positions registered",
  "Aucune pénalité enregistrée": "No penalties registered",
  "Aucun prêt enregistré": "No loans registered",
  "Aucun versement enregistré": "No disbursements registered",
  "Aucune cotisation enregistrée": "No contributions registered",
  "Aucune liaison enregistrée": "No links registered",
  "Aucune attribution enregistrée": "No assignments registered",
  "Aucune donnée disponible": "No data available",
  "Aucun événement trouvé": "No events found",

  // KPI labels
  "Total membres": "Total Members",
  "Total tontines": "Total Tontines",
  "Total cycles": "Total Cycles",
  "Total versements": "Total Disbursements",
  "Total cotisations": "Total Contributions",
  "Total prêts": "Total Loans",
  "Total comptes": "Total Accounts",
  "Comptes actifs": "Active Accounts",
  "Administrateurs": "Administrators",
  "Inactifs / Suspendus": "Inactive / Suspended",
  "Aujourd'hui": "Today",
  "Ces 7 derniers jours": "Last 7 days",
  "Total événements": "Total Events",
  "Prêts en retard": "Overdue Loans",
  "Cotisations impayées": "Unpaid Contributions",

  // Sections
  "Données de base": "Base Data",
  "Liaisons": "Links",
  "Remplissez les informations": "Fill in the information",
  "Cette action est irréversible": "This action is irreversible",
  "Êtes-vous sûr de vouloir supprimer cette liaison ?": "Are you sure you want to delete this link?",
  "Historique des activités": "Activity History",
  "Activités Récentes": "Recent Activities",
  "Liste des comptes": "Account List",
  "Retour à la liste": "Back to list",

  // Topbar
  "Mode sombre": "Dark mode",
};

// Dictionnaire inverse EN→FR pour revenir au français
const AUTO_TRANSLATE_EN_TO_FR = Object.fromEntries(
  Object.entries(AUTO_TRANSLATE).map(([fr, en]) => [en, fr])
);

// ─── Chargement des traductions (fichiers JSON) ───────────────────────────────
async function loadTranslations(language) {
  try {
    const response = await fetch(`/locales/${language}.json`);
    if (!response.ok) return false;
    translations = await response.json();
    currentLanguage = language;
    localStorage.setItem(LANGUAGE_KEY, language);
    return true;
  } catch (e) {
    console.error('i18n load error:', e);
    return false;
  }
}

function t(key, defaultValue = key) {
  return translations[key] || defaultValue;
}

// ─── Translation via attributs data-i18n ─────────────────────────────────────
function applyDataAttributes() {
  document.querySelectorAll('[data-i18n]').forEach(el => {
    const key = el.getAttribute('data-i18n');
    const val = t(key);
    if (val && val !== key) el.textContent = val;
  });
  document.querySelectorAll('[data-i18n-title]').forEach(el => {
    const key = el.getAttribute('data-i18n-title');
    const val = t(key);
    if (val && val !== key) el.title = val;
  });
}

// ─── Translation automatique du DOM ──────────────────────────────────────────
function applyAutoTranslate() {
  const dict = currentLanguage === 'en' ? AUTO_TRANSLATE : AUTO_TRANSLATE_EN_TO_FR;
  if (currentLanguage === 'fr') return; // FR est la langue source, pas besoin

  // Éléments UI ciblés (pas les données)
  const selectors = [
    'h2', 'h3', 'h4',
    'th',
    'label',
    '.kpi-lbl',
    '.menu-sublabel',
    '.topbar-title',
    'button:not([data-i18n])',
    'a.btn, a[style*="border-radius"][style*="padding"]',
  ];

  selectors.forEach(sel => {
    document.querySelectorAll(sel).forEach(el => {
      translateElement(el, dict);
    });
  });

  // Boutons spéciaux (contiennent une icône + du texte)
  document.querySelectorAll('button, a').forEach(el => {
    // Ne pas toucher aux éléments de navigation (déjà gérés par data-i18n)
    if (el.closest('.sidebar')) return;
    translateElementMixed(el, dict);
  });

  // Badges de statut et labels KPI
  document.querySelectorAll('span, div.kpi-lbl, td').forEach(el => {
    if (el.children.length === 0) {
      translateElement(el, dict);
    }
  });
}

// Traduit un élément sans enfants HTML
function translateElement(el, dict) {
  const text = el.textContent.trim();
  if (dict[text]) {
    el.textContent = dict[text];
  }
}

// Traduit les noeuds texte d'un élément mixte (icône + texte)
function translateElementMixed(el, dict) {
  el.childNodes.forEach(node => {
    if (node.nodeType === Node.TEXT_NODE) {
      const text = node.textContent.trim();
      if (text && dict[text]) {
        node.textContent = node.textContent.replace(text, dict[text]);
      }
    }
  });
}

// ─── Mise à jour du sélecteur de langue ──────────────────────────────────────
function updateLanguageSelector(language) {
  const sel = document.getElementById('language-selector');
  if (sel) sel.value = language;
}

// ─── Application complète ─────────────────────────────────────────────────────
function applyTranslations() {
  applyDataAttributes();
  applyAutoTranslate();
  // Ré-initialiser les icônes Lucide si présentes
  if (window.lucide) lucide.createIcons();
}

// ─── Changement de langue ────────────────────────────────────────────────────
async function changeLanguage(language) {
  const success = await loadTranslations(language);
  if (success) {
    applyTranslations();
    updateLanguageSelector(language);
    // Stocker pour les prochaines pages
    localStorage.setItem(LANGUAGE_KEY, language);
  }
}

// ─── Initialisation ──────────────────────────────────────────────────────────
async function initLanguage() {
  const saved = localStorage.getItem(LANGUAGE_KEY) || DEFAULT_LANGUAGE;
  const success = await loadTranslations(saved);
  if (success) {
    applyTranslations();
    updateLanguageSelector(saved);
  }
}

window.onLanguageChange = async function (selector) {
  await changeLanguage(selector.value);
};

document.addEventListener('DOMContentLoaded', initLanguage);
if (document.readyState !== 'loading') initLanguage();
