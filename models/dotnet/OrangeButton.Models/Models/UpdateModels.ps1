nswag run .\NSwagConfig.nswag  

## fix System
(Get-Content .\ModelGenerated.cs) `
    -replace 'System.CodeDom.Compiler.GeneratedCode', 'global::System.CodeDom.Compiler.GeneratedCode' `
    -replace 'using System = global::System;', '' `
    -replace 'System.Collections.Generic.', 'global::System.Collections.Generic.' `
    -replace 'System.Collections.ObjectModel.', 'global::System.Collections.ObjectModel.' `
    -replace 'IEnumerable<global::System.Collections.Generic.List<(.*)>', 'IEnumerable<$1' `
    | Set-Content .\ModelGenerated.cs


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