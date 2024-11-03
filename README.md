# End Point Commerce

This solution contains the source code for the backend apps for the End Point Commerce store.

## Projects

The solution is organized in a simple structure inspired by Clean Architecture and Domain Driven Design.

The projects are:

1. **EndPointCommerce.Domain**: A class library project that contains the business logic.
2. **EndPointCommerce.Infrastructure**: A class library project that contains components for interacting with database and other system software as well as third party software and (web) APIs.
3. **EndPointCommerce.WebApi**: An ASP.NET Web API project that contains a REST API meant to be consumed by a frontend, user-facing app.
4. **EndPointCommerce.AdminPortal**: An ASP.NET Razor Pages Web app project meant for site administrators to manage various aspects of the store.
5. **EndPointCommerce.UnitTests**: An xUnit project that contains unit tests for all the projects in the solution. Every project has its own directory where their respective unit tests live. Unit tests are ones that test the classes and methods in isolation, without interacting with any external components.
6. **EndPointCommerce.IntegrationTests**: An xUnit project that contains integration tests of any level. All tests that interact with the database, external services, or other system infrastructure go here.

## About the EndPointCommerce.AdminPortal project

### Managing frontend dependencies

For managing JavaScript packages, this project uses the [PNPM](https://pnpm.io/) package manager. The scope of PNPM is restricted to the `wwwroot` directory. That's where the `package.json` file, the `pnpm-lock.yaml` file and the `node_modules` directory live. Along with any JS scripts, CSS styles, and any other static files that need to be available to the browser.

This means that all node commands need to be run from within the `wwwroot` directory. `wwwroot` is effectively the root of a node project.

There's no JS building process. This structure is in place only to manage third party libraries and their versions in a sane way.

To install all dependencies, run:

```sh
cd EndPointCommerce.AdminPortal/wwwroot
pnpm install
```

### Building the CSS stylesheets

This projects uses SCSS to define styling rules. In order to be served in a manner that the browser can use it, the SCSS needs to be compiled into plain CSS. This project uses [SASS](https://sass-lang.com/) to do so. SASS is installed as a package with PNPM and a build command is defined in the `package.json` file. In order to build the CSS, run this:

```sh
cd EndPointCommerce.AdminPortal/wwwroot
pnpm run sass
```

This command will:

1. Compile the source SCSS files, which are located in the project's `Stylesheets` directory.
2. Copy the results into `wwwroot/css/site.css`. This is the file that the HTML layout includes.

## About the EndPointCommerce.UnitTests project

All tests can be run with `dotnet test`.

For more verbose output, you can use `dotnet test -l "console;verbosity=normal"`.

## Tools

This code base makes use of the following dotnet tools:

1. dotnet-aspnet-codegenerator
2. dotnet-ef

## Containerized development environment

This solution has [Dev Containers](https://containers.dev/) configuration in place. This means that a containerized development environment can be spun up and used to develop and test the apps and libraries. The relevant files are located under the `.devcontainer` directory.

In order to run a Dev Container, you need:

* [Docker](https://www.docker.com/)
* [Docker Compose](https://docs.docker.com/compose/)
* [VS Code](https://code.visualstudio.com/)
* [VS Code's Dev Containers extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers).

Once you have all that installed, set the proper environment variables in `.devcontainer/docker-compose.yml`.

For the "db" service, under `services -> db -> environment`, these need to be set: `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD`.

> Note: Currently, there are development defaults hardcoded here.

Finally, open the project directory in VS Code and run the "Dev Containers: Reopen in Container" command via the Command Palette (Ctrl+Shift+P).

That will download and build the necessary images and containers: one for the PostgreSQL database and one with .NET 8 for running and debugging the apps themselves. Once done, VS Code will connect to the app container and you'll be able to develop in the container as if it was your host machine. NodeJS, along with PNPM, as well as the dotnet-aspnet-codegenerator and dotnet-ef tools will also be installed without needing any extra steps.

On first run, it'll be necessary to create the database and apply all migrations with `dotnet ef database update`. This will work if run from within the `EndPointCommerce.WebApi` or `EndPointCommerce.AdminPortal` directories.
