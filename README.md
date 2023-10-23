# Unpack DAC Package .NET Tool

Extracts dacpac and generates deployment script to target folder without need for actual database running.

## Install

```PowerShell
dotnet tool install --global UnpackDacPac
```

## Usage

`unpackdacpac unpack <dacPacPath> <outputPath> [OPTIONS]`

## Example

```PowerShell
unpackdacpac unpack Source.dacpac TargetPath  --deploy-script-exclude-object-type Users --deploy-script-exclude-object-type Logins --deploy-script-exclude-object-type RoleMembership
```

Will in `TargetPath` result in a files

- DacMetadata.xml
- Deploy.sql - *generated deployment script*
- model.sql - *formated sql*
- model.xml
- Origin.xml
- postdeploy.sql *if exists*

## unpack Options

```
-h, --help                                                      Prints help information
    --clean-output-path                                         Flag for if output path should be cleaned before
                                                                extraction
    --target-database                                           Target database name used for deploy script, if not
                                                                specified DAC Package name without extension is used
    --deploy-create-new-database                                Flag that specifies whether the existing database will be
                                                                dropped and a new database created before proceeding with
                                                                the actual deployment actions. Acquires single-user mode
                                                                before dropping the existing database (default false)
    --deploy-script-database-options                            Deployment script flag that specifies whether the
                                                                database options in the target database should be updated
                                                                to match the source model (default false)
    --deploy-script-ignore-permissions                          Deployment script flag that specifies whether to exclude
                                                                all permission statements from consideration when
                                                                comparing the source and target model (default false)
    --deploy-script-exclude-object-type <EXCLUDEDOBJECTTYPE>    Deployment script setting to exclude object types
```