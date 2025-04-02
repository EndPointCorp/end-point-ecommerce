# End Point Commerce

This solution contains the source code for the backend apps for the End Point Commerce store.

[![pipeline status](https://bits.endpointdev.com/end-point-open-source/end-point-commerce/badges/main/pipeline.svg)](https://bits.endpointdev.com/end-point-open-source/end-point-commerce/-/commits/main)

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

## CI

CI is setup with GitLab CI. The pipeline is defined in the `.gitlab-ci.yml` file. For pushes to branches and tags, the pipeline runs tests, builds docker images from `Dockerfile.{AdminPortal,Maintenance,Webapi}`, and pushes them to gitlab container registry associated with this repo.

## CI/CD Variables

Gitlab CI/CD variables are used to store sensitive info like API keys, the following should be defined in `Settings -> CI/CD -> Variables`:
- `TAX_JAR_API_KEY` - Sandbox API key for TaxJar for use by tests


## Container Registry

CI pushes the following docker images to the gitlab container registry at `bits.endpointdev.com:5050/end-point-open-source/end-point-commerce/`:
- `webapi` - The WebApi project running on port 8080
- `adminportal` - The AdminPortal running on port 8080
- `maintenance` - For dev related tasks

## Deployment

If you're trying to deploy this project somewhere, or you're just trying to run it locally without a full development environment ready, you can make use of the Docker Compose file `compose.yml` found in the root directory.

### How to run a new deployment for the first time

#### 1. Clone the repository

The easiest way to get started is just to clone this repository to wherever you intend to run it from:

```text
git clone git@bits.endpointdev.com:end-point-open-source/end-point-commerce.git
```

Or if you prefer, you can of course clone it somewhere else and manually copy it to wherever you intend to run it from.

#### 2. Update the secrets

Modify the secrets found under `/secrets`:

1. Set `postgres-db-password.txt` to a secure password that will be used for the Postgres database server's `postgres` user.
2. Set `end-point-commerce-db-password.txt` to a secure password for the user that will be used by the EndPointCommerce services to connect to the Postgres database.
3. Update the connection string found in `end-point-commerce-db-connection-string.txt` to ensure it is correct for your environment. In the majority of cases, all you will need to do is simply copy the password from the previous `end-point-commerce-db-password.txt` file, at put it at the end of the connection string here.

#### 3. Set up `.env`

Copy `/env.template` to `/.env` and modify it as appropriate for your environment. For example, you may want to adjust ports if you wish, and you will most likely need to update the various URLs except if you are just running locally to test.

#### 4. Start the services

To start up all the services and build the images run:

```text
docker compose up -d
```

This will start them all up in the background. You can follow the logs live while everything starts up by running:

```text
docker compose logs -f
```

Please note that with the way the `compose.yml` is out-of-the-box in this repository, the images that will be run are configured to be built locally from the various `Dockerfile`s that are also found in the root of this repository. The images you end up running will be directly built from the current commit of this repository that you have cloned. Most "real world" production deployments would likely wish to use tagged images built and pushed elsewhere (such as via a CI/CD pipeline). To do this, you will need to make some simple modifications to the `compose.yml`.

#### 5. Run database migrations

The Docker Compose file provides a "Maintenance" container as a convenience which can be used to do various maintenance tasks, such as applying database migrations. **Database migrations are NOT run automatically by the services at startup!** You _must_ run them manually.

To run the migrations simply run:

```text
docker compose exec maintenance run-migrations.sh
```

After a short wait, they should complete successfully.

If this is the first time you've run the migrations, you will likely see the following error:

```text
Failed executing DbCommand (16ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
SELECT migration_id, product_version
FROM "__EFMigrationsHistory"
ORDER BY migration_id;
```

As long as you see one or more `Applying migration ...` lines **after** this error, you can safely ignore it for the first run, as it is expected (the table in question doesn't exist until the migrations have been run at least once). It shouldn't happen again on subsequent runs.

#### 6. Verify everything works

Assuming you made no changes to the `.env` variables regarding ports or URLs, you should now be able to access the running services via the following URLs:

- **WebApi**: http://localhost:8002/
  This is a base API URL though, and there is nothing there (it will 404).
- **WebApi Swagger**: http://localhost:8002/swagger/
- **AdminPortal**: http://localhost:8001/

The default username is `epadmin` and the default password is `Password123`.

### Updating the deployment

Updating an existing deployment with an updated version more or less follows the steps outlined above, assuming you haven't made significant modifications to the `compose.yml` to suit your particular environment.

#### Stop the services

Stop everything that is running currently:

```text
docker compose down
```

#### Pull your changes

Either run `git pull` or re-copy your updates to your deployment environment however you prefer in your particular environment.

#### Restart the services

Now start everything back up again and re-build updated images (unless you have updated your `compose.yml` to pull tagged images from a repository, then you can of course omit the `--build` argument)

```text
docker compose up -d --build
```

#### Apply database migrations

If your update includes any database migrations, you should run them now:

```text
docker compose exec maintenance run-migrations.sh
```

You can also check if there are any pending database migrations that need to be run first if you prefer:

```text
docker compose exec maintenance check-migrations.sh
```
