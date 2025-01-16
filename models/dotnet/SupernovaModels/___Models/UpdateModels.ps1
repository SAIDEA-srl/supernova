#create a local copy of ob-models
Invoke-WebRequest -URI "https://raw.githubusercontent.com/Open-Orange-Button/Orange-Button-Taxonomy/676b4bb1730de5ed871668d652c9a738c47907ee/Master-OB-OpenAPI-2407-0-0.json" -OutFile '../Master-OB-OpenAPI-2407-0-0.json'
#replace remote reference with local copy
# (Get-Content ../Master-OB-OpenAPI-2407-0-0.json).Replace('#/components/schemas/', 'https://raw.githubusercontent.com/Open-Orange-Button/Orange-Button-Taxonomy/676b4bb1730de5ed871668d652c9a738c47907ee/Master-OB-OpenAPI-2407-0-0.json#/components/schemas/') | Set-Content  ../Master-OB-OpenAPI-2407-0-0.json
#create a local copy of supernova models
Copy-Item -Path '../../../../supernova-extensions-datamodel.json' -Destination '../'
#replace remote reference with local copy
(Get-Content ../supernova-extensions-datamodel.json).Replace('https://raw.githubusercontent.com/Open-Orange-Button/Orange-Button-Taxonomy/676b4bb1730de5ed871668d652c9a738c47907ee/', '') | Set-Content  ../supernova-extensions-datamodel.json
#create a local copy of ob-models
Copy-Item -Path '../../../../supernova-constants.json' -Destination '../'

nswag run .\NSwagConfig.nswag  

## fix System
(Get-Content .\ModelGenerated.cs) `
    -replace 'System.CodeDom.Compiler.GeneratedCode', 'global::System.CodeDom.Compiler.GeneratedCode' `
    -replace 'using System = global::System;', '' `
    -replace 'System.Collections.Generic.', 'global::System.Collections.Generic.' `
    -replace 'System.Collections.ObjectModel.', 'global::System.Collections.ObjectModel.' | Set-Content .\ModelGenerated.cs


## fix Parameters
#$replaces = @{ 
#    TaxonomyElementString       = 'string'
#    TaxonomyElementNumber       = 'double'
#    TaxonomyElementInteger      = 'int'
#    TaxonomyElementBoolean      = 'bool'
#    TaxonomyElementArrayNumber  = 'global::System.Collections.ObjectModel.Collection<double>'
#    TaxonomyElementArrayString  = 'global::System.Collections.ObjectModel.Collection<string>'
#    TaxonomyElementArrayBoolean = 'global::System.Collections.ObjectModel.Collection<bool>'
#    TaxonomyElementArrayInteger = 'global::System.Collections.ObjectModel.Collection<int>'
#}
#
#$fileContent = (Get-Content .\ModelGenerated.cs)
#
#foreach ($from in $replaces.Keys) {
#    $to = $replaces[$from]
#
#    $ms = $fileContent | Select-String "public partial class (.+) : $($from)" -AllMatches 
#    foreach ($match in $ms) {
#        $class = $match.Matches.Groups[1].Value
#        
#        $fileContent = $fileContent -replace "public $($class) $($class) { get; set; }", "public $($to) $($class) { get; set; }"
#    }
#}
#
#$fileContent | Set-Content .\ModelGenerated.cs