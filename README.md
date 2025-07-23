# Siren

[![nuget](https://github.com/gman-au/siren/actions/workflows/nuget.yml/badge.svg)](https://github.com/gman-au/siren/actions/workflows/nuget.yml)

![GitHub Release](https://img.shields.io/github/v/release/gman-au/siren)

## Summary
It is a simple command line tool that can be installed from NuGet.
When run, it will take a C# database migration context assembly, and create an entity relationship (ER) diagram in [Mermaid syntax](https://github.com/mermaid-js/mermaid).

## Usage
### Installation
You can install the `siren-gen` tool via the following .NET command
```
dotnet tool install -g Gman.Siren
```
### Running the tool
Run the command as follows:
```
siren-gen -a [--assemblyPath] <PATH_TO_MIGRATION_ASSEMBLY> -c [--connectionString] <DATABASE_CONNECTION_STRING> -o [--outputPath] <PATH_TO_OUTPUT_MARKDOWN_FILE> -m [--markdownAnchor] <MARKDOWN_ANCHOR>
```
- `PATH_TO_MIGRATION_ASSEMBLY` (optional, required if connection string not provided) - this will be the location of the built .NET DLL containing the database migration you wish to map to an ER diagram.
- `DATABASE_CONNECTION_STRING` (optional, required if assembly not provided) - this can be set to the connection string of a (currently online), accessible database. Siren uses the [SchemaSearch](https://github.com/gman-au/schema-search) library to run 
compatible [system information schema](https://learn.microsoft.com/en-us/sql/relational-databases/system-information-schema-views/system-information-schema-views-transact-sql?view=sql-server-ver17) queries using Entity Framework; this is another potential rendering option in the absence of a migration assembly.
- `PATH_TO_OUTPUT_MARKDOWN_FILE` (required) - this points to a file (on your local file system) where the markdown should be generated; includes the full file name. The file _does not have to be a markdown_ (`.md`) file.
- `MARKDOWN_ANCHOR` (optional) - this is useful for updating `README.md` files that may be associated with your domain model (i.e. committed in a git repository). If the markdown anchor is specified, and the output file contains that markdown anchor (for example `"### My Domain Model Diagram"`), then the Siren tool will only add or replace any __existing__ diagram it finds under that anchor, and leave the rest of the document unmodified.

## Example Github action
-  An example workflow file that will update the `README.md` of a (non-master) branch whenever a change is detected in the `Migrations` folder - [Not.Again](https://github.com/gman-au/not-again/blob/master/.github/workflows/siren-gen.yml)
-  The resulting domain model section in the [`README.md`](https://github.com/gman-au/not-again?tab=readme-ov-file#domain-model)