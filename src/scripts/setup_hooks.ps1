# Define the Git hooks directory
$gitHooksDir = ".git/hooks"

# Ensure the Git hooks directory exists
if (-Not (Test-Path $gitHooksDir)) {
    Write-Host "Error: This is not a valid Git repository (missing .git directory)." -ForegroundColor Red
    exit 1
}

# Define the contents of the pre-push hook in Bash
$prePushContent = @"
#!/bin/bash
# Bash script to compress 'dataset' folder before pushing

dataset_dir="dataset"
zip_file="dataset.zip"

if [ -d "`$dataset_dir" ]; then
  echo "Compressing `$dataset_dir to `$zip_file..."
  if [ -f "`$zip_file" ]; then
    rm "`$zip_file"
  fi
  zip -r "`$zip_file" "`$dataset_dir"
  echo "Compression complete."

  # Stage the zip file for the commit
  git add "`$zip_file"
else
  echo "`$dataset_dir folder not found."
fi
"@

# Define the contents of the post-merge hook in Bash
$postMergeContent = @"
#!/bin/bash
# Bash script to decompress 'dataset.zip' after pulling

zip_file="dataset.zip"
dataset_dir="dataset"

if [ -f "`$zip_file" ]; then
  echo "Unzipping `$zip_file to `$dataset_dir..."
  if [ -d "`$dataset_dir" ]; then
    rm -rf "`$dataset_dir"
  fi
  unzip "`$zip_file" -d "`$dataset_dir"
  echo "Unzipping complete."
else
  echo "`$zip_file not found."
fi
"@

# Define the contents of the post-checkout hook in Bash (optional)
$postCheckoutContent = @"
#!/bin/bash
# Bash script to decompress 'dataset.zip' after checkout

zip_file="dataset.zip"
dataset_dir="dataset"

if [ -f "`$zip_file" ]; then
  echo "Unzipping `$zip_file to `$dataset_dir..."
  if [ -d "`$dataset_dir" ]; then
    rm -rf "`$dataset_dir"
  fi
  unzip "`$zip_file" -d "`$dataset_dir"
  echo "Unzipping complete."
else
  echo "`$zip_file not found."
fi
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
Create-HookFile "pre-push" $prePushContent
Create-HookFile "post-merge" $postMergeContent
Create-HookFile "post-checkout" $postCheckoutContent

# Add dataset folder to .gitignore if not already ignored
$gitIgnorePath = ".gitignore"
$datasetEntry = "dataset/*/*.fbx"

if (-Not (Test-Path $gitIgnorePath)) {
    New-Item -Path $gitIgnorePath -ItemType File
}

$gitIgnoreContent = Get-Content $gitIgnorePath
if ($datasetEntry -notin $gitIgnoreContent) {
    Add-Content -Path $gitIgnorePath -Value $datasetEntry
    Write-Host "Added 'dataset/' to .gitignore."
}
else {
    Write-Host "'dataset/' is already in .gitignore."
}

Write-Host "Git hooks setup completed successfully!"
