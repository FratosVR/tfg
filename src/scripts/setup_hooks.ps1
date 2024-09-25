# Define the Git hooks directory
$gitHooksDir = ".git/hooks"

Set-Location ..
Set-Location ..

# Ensure the Git hooks directory exists
if (-Not (Test-Path $gitHooksDir)) {
    Write-Host "Error: This is not a valid Git repository (missing .git directory)." -ForegroundColor Red
    exit 1
}

# Define the contents of the pre-push hook
$prePushContent = @"
#!/usr/bin/env pwsh
# PowerShell script to compress 'dataset' folder before pushing

`$datasetDir = "dataset"
`$zipFile = "dataset.zip"

if (Test-Path `$datasetDir) {
    Write-Host "Compressing `$datasetDir to `$zipFile..."
    if (Test-Path `$zipFile) {
        Remove-Item `$zipFile
    }
    Compress-Archive -Path `$datasetDir -DestinationPath `$zipFile
    Write-Host "Compression complete."

    # Stage the zip file for the commit
    git add `$zipFile
} else {
    Write-Host "`$datasetDir folder not found."
}
"@

# Define the contents of the post-merge hook
$postMergeContent = @"
#!/usr/bin/env pwsh
# PowerShell script to decompress 'dataset.zip' after pulling

`$zipFile = "dataset.zip"
`$datasetDir = "dataset"

if (Test-Path `$zipFile) {
    Write-Host "Unzipping `$zipFile to `$datasetDir..."
    if (Test-Path `$datasetDir) {
        Remove-Item -Recurse -Force `$datasetDir
    }
    Expand-Archive -Path `$zipFile -DestinationPath `$datasetDir
    Write-Host "Unzipping complete."
} else {
    Write-Host "`$zipFile not found."
}
"@

# Define the contents of the post-checkout hook (optional)
$postCheckoutContent = @"
#!/usr/bin/env pwsh
# PowerShell script to decompress 'dataset.zip' after checkout

`$zipFile = "dataset.zip"
`$datasetDir = "dataset"

if (Test-Path `$zipFile) {
    Write-Host "Unzipping `$zipFile to `$datasetDir..."
    if (Test-Path `$datasetDir) {
        Remove-Item -Recurse -Force `$datasetDir
    }
    Expand-Archive -Path `$zipFile -DestinationPath `$datasetDir
    Write-Host "Unzipping complete."
} else {
    Write-Host "`$zipFile not found."
}
"@

# Function to create the hook files
function Create-HookFile {
    param (
        [string]$fileName,
        [string]$content
    )

    $filePath = Join-Path $gitHooksDir $fileName
    Set-Content -Path $filePath -Value $content -Force
    # Make the hook executable
    Write-Host "Creating $fileName hook..."
    icacls $filePath /grant Everyone:F > $null
    Write-Host "$fileName hook created and made executable."
}

# Create the pre-push, post-merge, and post-checkout hooks
Create-HookFile "pre-push.ps1" $prePushContent
Create-HookFile "post-merge.ps1" $postMergeContent
Create-HookFile "post-checkout.ps1" $postCheckoutContent

# Add dataset folder to .gitignore if not already ignored
$gitIgnorePath = ".gitignore"
$datasetEntry = "dataset/"

if (-Not (Test-Path $gitIgnorePath)) {
    New-Item -Path $gitIgnorePath -ItemType File
}

$gitIgnoreContent = Get-Content $gitIgnorePath
if ($datasetEntry -notin $gitIgnoreContent) {
    Add-Content -Path $gitIgnorePath -Value "$datasetEntry`n"
    Write-Host "Added 'dataset/' to .gitignore."
}
else {
    Write-Host "'dataset/' is already in .gitignore."
}

Write-Host "Git hooks setup completed successfully!"
