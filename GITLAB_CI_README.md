## CI

CI is setup with GitLab CI. The pipeline is defined in the `.gitlab-ci.yml` file. For pushes to branches and tags, the pipeline runs tests, builds docker images from `Dockerfile.{AdminPortal,Maintenance,Webapi}`, and pushes them to gitlab container registry associated with this repo.

### Variables

Gitlab CI/CD variables are used to store sensitive info like API keys, they should be defined in `Settings -> CI/CD -> Variables`.

### Container Registry

CI pushes the following docker images to the gitlab container registry at `bits.endpointdev.com:5050/end-point-open-source/end-point-ecommerce/`:
- `webapi` - The WebApi project running on port 8080
- `adminportal` - The AdminPortal running on port 8080
- `maintenance` - For dev related tasks