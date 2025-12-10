# 2025 Dark Mode Style Guide for Web Applications

## 1. Prioritize User Experience and Accessibility
*   **Reduce Eye Strain:** Dark mode minimizes glare, especially in low-light environments.
*   **Enhance Readability:** Ensure high contrast for text to prevent strain, particularly for users with visual impairments.
*   **Improve Focus:** Dark backgrounds help users focus on content by reducing distractions.
*   **Energy Efficiency:** For OLED/AMOLED screens, dark mode conserves battery life.

## 2. Thoughtful Color Palette and Contrast
*   **Avoid Pure Black and Pure White:** Use deep greys (e.g., `#121212`, `#1b1b1b`) instead of pure black. Use off-white or light grey for text.
*   **Maintain High Contrast:** Adhere to WCAG 2.1 standards: a minimum contrast ratio of 4.5:1 for normal text and 3:1 for larger text.
*   **Desaturated Colors:** Use muted versions of your primary and accent colors to avoid being too intense.
*   **Strategic Accent Colors:** Use vibrant accent colors sparingly to highlight important elements like buttons and links.

## 3. Visual Hierarchy and Depth
*   **Communicate Depth Without Shadows:** Use subtle gradients, borders, or slightly lighter background shades for elevated layers.
*   **Consistent Spacing:** Maintain consistent spacing to guide the user's eye and prevent a cluttered interface.

## 4. Content and Imagery Adaptation
*   **Optimize Images and Media:** Adjust brightness/contrast or provide dark mode variants of images.
*   **Typography:** Use regular and medium font weights. Ensure text is slightly larger and well-spaced.
*   **Branding:** Reassess how your brand's primary colors and logos appear. Create dark mode variants if necessary.

## 5. User Control and System Preferences
*   **Offer a Toggle Option:** Always provide a toggle to switch between light and dark modes.
*   **Respect System Preferences:** Use the `prefers-color-scheme` CSS media query to automatically apply the user's system preference.
*   **Persist User Choice:** Store the user's theme selection (e.g., in `localStorage`) to remember their preference across sessions.

## 6. Technical Implementation Best Practices
*   **CSS Variables:** Use CSS variables (custom properties) for your color palettes for easy theme switching.
*   **Dynamic Switching:** Use JavaScript to toggle themes and detect system preference changes.
*   **Prevent "Flash of Wrong Theme" (FOWT):** For server-side rendered applications, ensure the dark mode is set before the application mounts.
*   **Smooth Transitions:** Implement smooth transitions when switching between themes.

## 7. Rigorous Testing
*   **Test in Real-World Conditions:** Test across various devices, screen sizes, and lighting conditions.
*   **Accessibility Tools:** Use tools like WebAIM Contrast Checker, Lighthouse, or axe-core to verify contrast ratios and ensure WCAG compliance.
*   **Gather User Feedback:** Collect feedback from users to make necessary adjustments.
