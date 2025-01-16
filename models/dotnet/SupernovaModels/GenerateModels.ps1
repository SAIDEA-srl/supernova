openapi-generator-cli generate -g csharp -i .\supernova-extensions-datamodel.json -o . -c openapi-generator-config.yml `
    --openapi-normalizer=REF_AS_PARENT_IN_ALLOF=true `
    --openapi-normalizer=REFACTOR_ALLOF_WITH_PROPERTIES_ONLY=true `
    --global-property=models=Device `
    --language-specific-primitives=Location `
    --import-mappings=Location=OrangeButton.Models.Location `
    --type-mappings=Location=Location `
    --enable-post-process-file `
    --verbose
