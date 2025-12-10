# Login UI Look & Feel (Atomic + eviCore Component Library)

Practical styling guidance to build a production-quality login screen that reuses the eviCore Component Library (`E:\ComponentLibrary\src\components`) and the existing atomic design approach in `Stargate.UI`.

## Atomic mapping for the login page
- **Atoms** (use existing implementations): `button.tsx`, `input.tsx`, `label.tsx`, `alert.tsx`, `popover-tooltip.tsx` from `components/atoms`; keep font sizes consistent with `atoms.css` (16px body text).
- **Molecules**: prefer `form-input.tsx`, `form-input-date.tsx`, `form-text-area.tsx` (if needed), `form-button.tsx`, `alert-control.tsx` from `components/molecules` to group labels/inputs/errors.
- **Organisms**: wrap the form in a simple card-like container; for modal flows use `modal-control.tsx` from `components/organisms`.
- **Templates/Pages**: page-level layout owns data fetching + validation; organisms stay presentation-focused.

## Brand tokens (from `eviCoreBootstrapTheme.css`)
- Primary `#34657f`; Info/links `#0e73a9`; Success `#238622`; Warning `#eac435`; Danger `#a82e2b`; Neutral text `#495057`; Light surface `#f8f9fa`; Raised surface `#f1f2f3`; White `#ffffff`.
- Typography stack matches Bootstrap defaults: `"Segoe UI", Roboto, "Helvetica Neue", Arial, "Noto Sans", sans-serif`. Use 600 weight for headings, 500 for labels.
- Spacing on an 8px grid; default line-height ~1.5. Buttons/inputs: 12px vertical padding, 14–16px horizontal padding.

## Layout and composition
- **Viewport**: center the card vertically/horizontally with ample breathing room; full-height background using `#f8f9fa`.
- **Card**: 420–480px width, `border-radius: 6px`, subtle shadow (`0 6px 24px rgba(0,0,0,0.08)`), surface `#ffffff`.
- **Header**: page title + short supporting text to set expectation; align left.
- **Form flow**: email/username first, password second, optional MFA input third. Keep consistent vertical rhythm (24px between groups).
- **Actions**: primary submit uses `btn btn-primary` (maps to `#34657f`), secondary text link for “Forgot password?” using `#0e73a9`.
- **Error zone**: place `alert-control` or `alert` above the form; inline field errors directly under inputs.

## States and affordances
- **Focus**: visible focus ring; if customizing, use `outline: 2px solid #0e73a9` with 2px offset.
- **Hover**: slightly darken primary (`#2c556a`) and link (`#084362`), reduce box-shadow on press for tactile feedback.
- **Disabled**: lower opacity to 0.6, keep label text legible; do not remove focus ring.
- **Validation**: show error text in `#a82e2b`, border `1px solid #a82e2b` on invalid fields; success border `#238622` when needed.
- **Loading**: if auth call in-flight, show inline spinner in the button and disable inputs to prevent double submits.

## Accessibility and content
- Use descriptive labels, `aria-describedby` for error/help text, `aria-invalid` on errored inputs.
- Keyboard: tab order Email → Password → Remember me → Submit → Forgot password.
- Copy tone: concise and task-focused (“Sign in to Stargate” / “Continue”); avoid dense paragraphs.

## Suggested structure (pseudo-markup)
```
<page background="#f8f9fa">
  <card>
    <h1>Sign in</h1>
    <p>Access Stargate with your enterprise credentials.</p>
    <alert v-if="authError">…</alert>
    <form>
      <form-input label="Email" type="email" required />
      <form-input label="Password" type="password" required />
      <form-check-box label="Remember me" />
      <button kind="primary" block>Continue</button>
      <a class="mt-2" href="#">Forgot password?</a>
    </form>
  </card>
</page>
```

## Dark mode alignment (optional)
- Reuse `documents/dark_mode_style_guide.md`: base `#121212`, surface `#1b1b1b`, text `#e6e6e6`, accent `#0e73a9` (muted to 85% saturation), border `#2a2a2a`.
- Keep card shadow minimal; prefer subtle borders to separate layers.

## Quality checklist (production-level)
- Contrast ratios ≥ 4.5:1 for body text; tooltips accessible on keyboard focus.
- Loading + error states covered; API errors mapped to inline alerts.
- Responsive: card max-width 480px with 24px gutters on mobile; stack actions vertically.
- Consistency: use Component Library atoms/molecules only; avoid ad-hoc styles except layout wrappers.
