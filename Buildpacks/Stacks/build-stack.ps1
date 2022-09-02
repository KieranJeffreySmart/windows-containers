param (
    [string]$path = '.',
    [string]$prefix = 'cnbs/sample-stack',
    [string]$platform = 'amd64'
)

$ID_PREFIX="io.buildpacks.samples.stacks"

function Write-Usage() {
  Write-Output "Usage: "
  Write-Output "  $0 [-f <prefix>] [-p <platform>] <dir>"
  Write-Output "    -f    prefix to use for images      (default: $prefix)"
  Write-Output "    -p    platform to use for images      (default: $platform)"
  Write-Output "   <dir>  directory of stack to build"
  exit 1; 
}

if([string]::IsNullOrEmpty($prefix)) {
  Write-Output "Prefix cannot be empty"
  Write-Output ""
  Write-Usage
  exit 1
}

if([string]::IsNullOrEmpty($path)) {
  Write-Output "Must specify stack directory"
  Write-Output ""
  Write-Usage
  exit 1
}

$FULL_PATH = Resolve-Path $path
$STACK_NAME=[System.IO.Path]::GetFileName($FULL_PATH)
$STACK_ID="$ID_PREFIX.$STACK_NAME"
$BASE_IMAGE="$prefix-base:$STACK_NAME"
$RUN_IMAGE="$prefix-run:$STACK_NAME"
$BUILD_IMAGE="$prefix-build:$STACK_NAME"



if ([System.IO.File]::Exists("$path/base")) {
docker build --platform=$platform -t $BASE_IMAGE $path/base
}

Write-Output "BUILDING $BUILD_IMAGE..."
docker build --platform=$platform --build-arg base_image=$BASE_IMAGE --build-arg stack_id=$STACK_ID -t $BUILD_IMAGE  $path/build

Write-Output "BUILDING $RUN_IMAGE..."
docker build --platform=$platform --build-arg base_image=$BASE_IMAGE --build-arg stack_id=$STACK_ID -t $RUN_IMAGE $path/run

Write-Output ""
Write-Output "STACK BUILT!"
Write-Output ""
Write-Output "Stack ID: $STACK_ID"
Write-Output "Images:"
foreach ($IMAGE in @($BASE_IMAGE, $BUILD_IMAGE, $RUN_IMAGE)) {
    Write-Output "    $IMAGE"
}