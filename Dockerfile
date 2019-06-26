FROM dcreg.service.consul/prod/development-dotnet-core-sdk-common:latest

# build scripts
COPY ./fake.sh /fstate/
COPY ./build.fsx /fstate/
COPY ./paket.dependencies /fstate/
COPY ./paket.references /fstate/
COPY ./paket.lock /fstate/

# sources
COPY ./State.fsproj /fstate/
COPY ./src /fstate/src

WORKDIR /fstate

RUN \
    ./fake.sh build target Build no-clean

CMD ["./fake.sh", "build", "target", "Tests", "no-clean"]
