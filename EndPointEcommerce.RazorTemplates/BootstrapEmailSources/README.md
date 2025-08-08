The *.html files in this directory are the raw sources used to generate the *.cshtml Razor templates found under `EndPointEcommerce.RazorTemplates/Views/Emails`. [Bootstrap Email](https://bootstrapemail.com/) was used to generate the templates.

With help from this tool, the process to create the templates is laborious but straightforward:

1. The source files, which are plain HTML that use [a subset of the Bootstrap CSS classes](https://bootstrapemail.com/docs/introduction), are fed one by one into the [Bootstrap Email web app](https://app.bootstrapemail.com/).
2. Bootstrap Email produces plain HTML files with inline styling. These are compatible with many email clients.
3. The rendered output is carefully analyzed to extract the main layout (`EndPointEcommerce.RazorTemplates/Views/Shared/EmailLayout.cshtml`) and the contents of the individual email templates (The *.cshtml files under `EndPointEcommerce.RazorTemplates/Views/Emails`).
4. These files are then manually edited with the necessary Razor syntax to display the relevant order and account information, depending on each particular template's needs.
