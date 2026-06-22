/* tableutils.js — Pagination + Export CSV for list tables */
(function (w) {
    'use strict';

    function Pager(cfg) {
        this.cls = cfg.rowClass;
        this.ps  = cfg.pageSize || 15;
        this.bar = cfg.barParent;
        this._p  = 1;
        this._build();
        this.refresh();
    }

    Pager.prototype = {
        _all: function () { return [].slice.call(document.querySelectorAll('.' + this.cls)); },
        _vis: function () { return this._all().filter(function (r) { return !r.classList.contains('th-f'); }); },

        search: function (q) {
            q = (q || '').toLowerCase().trim();
            this._p = 1;
            this._all().forEach(function (r) {
                r.classList.toggle('th-f', q.length > 0 && !(r.dataset.search || '').toLowerCase().includes(q));
            });
            this.refresh();
        },

        goPage: function (n) {
            var tp = Math.max(1, Math.ceil(this._vis().length / this.ps));
            this._p = Math.min(Math.max(1, n), tp);
            this.refresh();
        },

        refresh: function () {
            var vis  = this._vis();
            var from = (this._p - 1) * this.ps;
            var to   = from + this.ps;
            this._all().forEach(function (r) {
                if (r.classList.contains('th-f')) { r.style.display = 'none'; return; }
                r.style.display = vis.indexOf(r) >= from && vis.indexOf(r) < to ? '' : 'none';
            });
            this._upd(vis.length);
        },

        csv: function (headers, fn, name) {
            var lines = [headers.map(function (h) { return '"' + h + '"'; }).join(';')];
            this._vis().forEach(function (r) {
                lines.push(fn(r).map(function (v) {
                    return '"' + String(v == null ? '' : v).replace(/"/g, '""') + '"';
                }).join(';'));
            });
            var a = document.createElement('a');
            a.href = URL.createObjectURL(new Blob(['﻿' + lines.join('\r\n')], { type: 'text/csv;charset=utf-8;' }));
            a.download = (name || 'export') + '.csv';
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
        },

        _build: function () {
            var self = this;
            var parent = document.getElementById(this.bar);
            if (!parent) return;
            var d = document.createElement('div');
            d.id = 'pgb-' + this.cls;
            d.style.cssText = 'display:none;align-items:center;justify-content:space-between;padding:11px 18px;background:#F8FDF9;border-top:1px solid #E8F5EE;flex-wrap:wrap;gap:6px;border-radius:0 0 12px 12px;';
            d.innerHTML =
                '<span id="pgi-' + this.cls + '" style="font-size:12px;color:#666;"></span>' +
                '<div style="display:flex;align-items:center;gap:6px;">' +
                '<button id="pgp-' + this.cls + '" style="background:#F4F6F9;border:1px solid #E0E0E0;border-radius:6px;padding:4px 14px;cursor:pointer;font-size:15px;color:#555;line-height:1.4;">&#8249;</button>' +
                '<span id="pgn-' + this.cls + '" style="font-size:12px;color:#444;min-width:90px;text-align:center;"></span>' +
                '<button id="pgx-' + this.cls + '" style="background:#0F6E56;color:white;border:none;border-radius:6px;padding:4px 14px;cursor:pointer;font-size:15px;line-height:1.4;">&#8250;</button>' +
                '</div>';
            parent.appendChild(d);
            document.getElementById('pgp-' + this.cls).onclick = function () { self.goPage(self._p - 1); };
            document.getElementById('pgx-' + this.cls).onclick = function () { self.goPage(self._p + 1); };
        },

        _upd: function (total) {
            var d = document.getElementById('pgb-' + this.cls);
            if (!d) return;
            var tp   = Math.max(1, Math.ceil(total / this.ps));
            var from = Math.min((this._p - 1) * this.ps + 1, total || 0);
            var to   = Math.min(this._p * this.ps, total);
            d.style.display = total > this.ps ? 'flex' : 'none';
            var el = function (id) { return document.getElementById(id); };
            el('pgi-' + this.cls).textContent = total > 0 ? from + '–' + to + ' sur ' + total + ' enregistrement(s)' : 'Aucun résultat';
            el('pgn-' + this.cls).textContent = 'Page ' + this._p + ' / ' + tp;
            var prev = el('pgp-' + this.cls); prev.disabled = this._p <= 1; prev.style.opacity = this._p <= 1 ? '0.4' : '1';
            var next = el('pgx-' + this.cls); next.disabled = this._p >= tp; next.style.opacity = this._p >= tp ? '0.4' : '1';
        }
    };

    w.TablePager = Pager;
})(window);

/* ─── Toast Notifications ────────────────────────────────────────────── */
(function (w) {
    'use strict';

    var COLORS = {
        success: { bg: '#0F6E56', icon: '✓', border: '#0B5940' },
        error:   { bg: '#C62828', icon: '✕', border: '#9B1B1B' },
        warning: { bg: '#E65100', icon: '⚠', border: '#BF360C' },
        info:    { bg: '#1565C0', icon: 'ℹ', border: '#0D47A1' }
    };

    function getContainer() {
        var c = document.getElementById('toast-container');
        if (!c) {
            c = document.createElement('div');
            c.id = 'toast-container';
            c.style.cssText = 'position:fixed;bottom:24px;left:50%;transform:translateX(-50%);z-index:99999;display:flex;flex-direction:column;align-items:center;gap:8px;pointer-events:none;';
            document.body.appendChild(c);
        }
        return c;
    }

    w.showToast = function (message, type) {
        type = type || 'success';
        var cfg = COLORS[type] || COLORS.success;
        var container = getContainer();

        var toast = document.createElement('div');
        toast.style.cssText =
            'background:' + cfg.bg + ';color:white;border-left:4px solid ' + cfg.border + ';' +
            'border-radius:10px;padding:12px 20px;font-size:14px;font-weight:500;' +
            'box-shadow:0 4px 20px rgba(0,0,0,0.25);display:flex;align-items:center;gap:10px;' +
            'pointer-events:auto;min-width:260px;max-width:420px;' +
            'animation:toast-in 0.3s cubic-bezier(0.34,1.56,0.64,1) forwards;';

        var iconSpan = document.createElement('span');
        iconSpan.style.cssText = 'font-size:16px;font-weight:700;flex-shrink:0;';
        iconSpan.textContent = cfg.icon;

        var textSpan = document.createElement('span');
        textSpan.style.cssText = 'flex:1;line-height:1.4;';
        textSpan.textContent = message;

        var closeBtn = document.createElement('button');
        closeBtn.style.cssText = 'background:none;border:none;color:rgba(255,255,255,0.7);font-size:16px;cursor:pointer;padding:0 0 0 8px;line-height:1;flex-shrink:0;';
        closeBtn.textContent = '×';

        toast.appendChild(iconSpan);
        toast.appendChild(textSpan);
        toast.appendChild(closeBtn);
        container.appendChild(toast);

        function remove() {
            toast.style.animation = 'toast-out 0.25s ease forwards';
            setTimeout(function () { if (toast.parentNode) toast.parentNode.removeChild(toast); }, 260);
        }

        closeBtn.onclick = remove;
        setTimeout(remove, 3500);
    };
})(window);
