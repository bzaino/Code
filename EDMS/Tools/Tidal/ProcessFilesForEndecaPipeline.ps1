
[CmdletBinding()]
Param
(
    [Parameter(Mandatory=$true)]
    [ValidateSet("PRD","STA","TST","DEV", "PER", "AUT")]
    [string]$Environment,

    [Parameter(Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [string]$InsightFileName,

    [Parameter(Mandatory=$true,ValueFromPipeline=$true)]
    [ValidateScript({Test-Path $_ -PathType Container})]
    [string]$InsightFileDirectory,

    [Parameter(Mandatory=$true,ValueFromPipeline=$true)]
    [ValidateScript({Test-Path $_ -PathType Container})]
    [string]$ArticleFileDirectory,

    [Parameter(Mandatory=$true,ValueFromPipeline=$true)]
    [ValidateScript({Test-Path $_ -PathType Leaf})]
    [string[]]$OrderedBatFilesToRun,

    [Parameter(Mandatory=$true)]
    [ValidateScript({Test-Path $_ -PathType Container})]
    [string]$ArchiveDirectory,

    [Parameter(Mandatory=$true)]
    [ValidateScript({Test-Path $_ -PathType Container})]
    [string]$ErrorDirectory,

    [Parameter(Mandatory=$false)]
    [ValidateNotNullOrEmpty()]
    [string[]]$EmailToSuccessAndFailure,

    [Parameter(Mandatory=$false)]
    [ValidateNotNullOrEmpty()]
    [string[]]$EmailToFailureOnly,

    [Parameter(Mandatory=$false)]
    [ValidateScript({Test-Path $_ -PathType Container})]
    [string]$LogDirectory,

    [Parameter(Mandatory=$true)]
    [ValidateScript({Test-Path $_ -PathType Container})]
    [string]$SaveCurrentContentRootDirectory,

    [Parameter(Mandatory=$false)]
    [switch]$ProcessWithoutComparingLastSessionContent
)
begin{
    $ErrorActionPreference = "Stop";}

process{

    #Function to compare content files between current session and last session
    function local:ContentDifferenceFoundFromLastSession () {
        #Get path to latest archive folder, replace current content folder with files from current session
        $lLastSessionArchiveFolder = Get-ChildItem -Path $ArchiveDirectory | sort -Descending -Property LastWriteTimeUtc | select -First 1
        [ASA.Task.Contracts.LogHelper]::WriteLineInfo([string]::Format("PS - Comparing files from current session with last session archived to directory '{0}'",$lLastSessionArchiveFolder.Name),$false);
        $CurrentContentFolderName = "CurrentContent";
        $CurrentContentFolderPath = [System.IO.DirectoryInfo](Join-Path $SaveCurrentContentRootDirectory $CurrentContentFolderName);
        if ($CurrentContentFolderPath.Exists){
            [void]$CurrentContentFolderPath.Delete($true);
            [ASA.Task.Contracts.LogHelper]::WriteLineInfo([string]::Format("PS - Directory '{0}' and its content deleted to make room for copying current session files.",$CurrentContentFolderPath.FullName),$false);
        }
        
        [void] (XCopy-Item -CopyFromDir $ArticleFileDirectory -CopyToDir $CurrentContentFolderPath.FullName);
        [void] (Copy-SingleFile -SourceFileFullPath $InsightFileInfo.FullName -TargetDirectory $CurrentContentFolderPath.FullName);
        [ASA.Task.Contracts.LogHelper]::WriteLineInfo([string]::Format("PS - Current session files are copied to directory '{0}'.",$CurrentContentFolderPath.FullName),$false);
        
        #Compare current session files from latest archive folder files
        $lCompareFiles = Compare-Object -ReferenceObject (Get-ChildItem $CurrentContentFolderPath.FullName) -DifferenceObject (Get-ChildItem (Join-Path $ArchiveDirectory $lLastSessionArchiveFolder.Name)) -IncludeEqual
        $lNewFiles = $lCompareFiles | ?{$_.SideIndicator -eq "<="}
        $lDeletedFiles = $lCompareFiles | ?{$_.SideIndicator -eq "=>"}
        $lSameFiles = $lCompareFiles | ?{$_.SideIndicator -eq "=="}

        #New files in current session, not present from previous session
        if ($lNewFiles.Length -eq 0){
            $Action = " ";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
            $Action = "PS - NEW FILES COMPARED TO LAST SESSION: None";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
        } else {
            $script:AtLeastOneContentDifferenceFoundFromLastSession = $true;
            $Action = " ";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
            $Action = "PS - NEW FILES COMPARED TO LAST SESSION:";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
            $lNewFiles | %{$Action = $_.InputObject.Name;$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);}
        }

        #Files from previous session deleted in current session
        if ($lDeletedFiles.Length -eq 0){
            $Action = " ";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
            $Action = "PS - LIST OF FILES WHICH WERE PRESENT IN PREVIOUS RUN AND ABSENT FROM CURRENT RUN: None";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
        } else {
            $script:AtLeastOneContentDifferenceFoundFromLastSession = $true;
            $Action = " ";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
            $Action = "PS - LIST OF FILES WHICH WERE PRESENT IN PREVIOUS RUN AND ABSENT FROM CURRENT RUN:";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
            $lDeletedFiles | 
            %{
                $Action = $_.InputObject.Name;
                $Output.Action += $Action;
                [ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
             }
        }

        #Same files in current session as in previous session, but with modified content
        $Action = " ";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
        $Action = "PS - SAME FILES AS LAST SESSION BUT DIFFERENT CONTENT:";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
        $lCount = 0;
        $lSameFiles |
        %{
            $lContentLatestFile = Get-Content (Join-Path $CurrentContentFolderPath.FullName $_.InputObject.Name)
            $lContentSecondLatestFile = Get-Content (Join-Path $ArchiveDirectory (Join-Path $lLastSessionArchiveFolder.Name $_.InputObject.Name))
            $lCompareContent = Compare-Object -ReferenceObject $lContentLatestFile -DifferenceObject $lContentSecondLatestFile ;
            if ($lCompareContent.InputObject.Length -gt 0){
                $Action = " ";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
                $lCount++;
                $Action = $lCount.ToString() + ". " + $_.InputObject.Name;
                $Output.Action += $Action;
                [ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);

                $Action = " ";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
                $Action = "NEW CONTENT:";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
                $Action = " ";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);

                $lCompareContent | ?{$_.SideIndicator -eq "<="} | %{$Action = $_.InputObject;$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);}
                $Action = " ";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
                $Action = "DELETED CONTENT:";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
                $Action = " ";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
                $lCompareContent | ?{$_.SideIndicator -eq "=>"} | %{$Action = $_.InputObject;$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);}
                $Action = " ";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);

            }
        }
        if ($lCount -eq 0){$Action = "None";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);}
        else {$script:AtLeastOneContentDifferenceFoundFromLastSession = $true;}
    }

    #Initialize
    $EmailSubject = "$Environment - Endeca Pipeline Files Processing Status";
    $Output = @{'InsightFileExists'=$false;'AtLeastOneArticleFileExists'=$false;'Error'="NONE";'Action'=@();}
    $Action = "";

    #Load dependencies
    . \\amsa.com\Process\Scripts\$Environment\PS\LibFileFunctions.ps1
    . \\amsa.com\Process\Scripts\$Environment\PS\LibMiscFunctions.ps1

    #Prepare a logger
    if (![string]::IsNullOrEmpty($LogDirectory)){Initialize-LogFile -Environment $Environment -LogFolder $LogDirectory -CallerIdentifier "EndecaContentFiles";}
    [ASA.Task.Contracts.LogHelper]::WriteLineInfo("Initialized Log.");
    
    #Log input params to this script
    [ASA.Task.Contracts.LogHelper]::WriteLineInfo("Input params:");
    [ASA.Task.Contracts.LogHelper]::WriteLineInfo("Environment:" + $Environment,$false);
    [ASA.Task.Contracts.LogHelper]::WriteLineInfo("InsightFileName:" + $InsightFileName,$false);
    [ASA.Task.Contracts.LogHelper]::WriteLineInfo("InsightFileDirectory:" + $InsightFileDirectory,$false);
    [ASA.Task.Contracts.LogHelper]::WriteLineInfo("ArticleFileDirectory:" + $ArticleFileDirectory,$false);
    [ASA.Task.Contracts.LogHelper]::WriteLineInfo("OrderedBatFilesToRun:" + $OrderedBatFilesToRun,$false);
    [ASA.Task.Contracts.LogHelper]::WriteLineInfo("ArchiveDirectory:" + $ArchiveDirectory,$false);
    [ASA.Task.Contracts.LogHelper]::WriteLineInfo("ErrorDirectory:" + $ErrorDirectory,$false);
    [ASA.Task.Contracts.LogHelper]::WriteLineInfo("EmailToSuccessAndFailure:" + $EmailToSuccessAndFailure,$false);
    [ASA.Task.Contracts.LogHelper]::WriteLineInfo("EmailToFailureOnly:" + $EmailToFailureOnly,$false);
    [ASA.Task.Contracts.LogHelper]::WriteLineInfo("SaveCurrentContentRootDirectory:" + $SaveCurrentContentRootDirectory,$false);
    [ASA.Task.Contracts.LogHelper]::WriteLineInfo("ProcessWithoutComparingLastSessionContent:" + $ProcessWithoutComparingLastSessionContent,$false);

    #Cast to .NET types
    $InsightFileDirectoryInfo = [System.IO.DirectoryInfo]$InsightFileDirectory;
    $ArticleFileDirectoryInfo = [System.IO.DirectoryInfo]$ArticleFileDirectory;
    $InsightFileInfo = [System.IO.FileInfo]([string]::Format("{0}\{1}",$InsightFileDirectoryInfo.FullName,$InsightFileName))

    #Check if all required files exist
    $Action = " ";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
    $Action = "PS - C H E C K I N G   C O N T E N T   S T A T I C T I C S   &    A R T I C L E    F I L E S:";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action);
    $Action = "";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);

    if ($InsightFileInfo.Exists){$Output.InsightFileExists = $true;}
    if ($ArticleFileDirectoryInfo.GetFiles().Count -gt 0){$Output.AtLeastOneArticleFileExists = $true;}

    $ContinueArchiveAndProcessBATFiles = $False;

    if ($Output.InsightFileExists -and $Output.AtLeastOneArticleFileExists)
    {
        try
        {
            $Action = "PS - Content Statictics file and at least one Article file exist.";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
          
            #Process only if the files in this session are different from last session (files from last session are saved in latest DATED
            #archive folder). The only exception to this rule is if switch ProcessWithoutComparingLastSessionContent is provided
            if ($ProcessWithoutComparingLastSessionContent){$ContinueArchiveAndProcessBATFiles = $True;}
            if (!$ContinueArchiveAndProcessBATFiles){
                $AtLeastOneContentDifferenceFoundFromLastSession = $false;
                ContentDifferenceFoundFromLastSession;
                $ContinueArchiveAndProcessBATFiles=$AtLeastOneContentDifferenceFoundFromLastSession;
            };

            if ($ContinueArchiveAndProcessBATFiles) {
                
                #Archive all files (Content Statictics and article files) into a newly created DATED folder
                $Action = " ";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
                $Action = "PS - A R C H I V I N G    F I L E S:";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action);
                $Action = "";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
                $DateTimeStamp = Get-FormattedDateTimeStamp;
                XCopy-Item -CopyFromDir $ArticleFileDirectory -CopyToDir (Join-Path $ArchiveDirectory $DateTimeStamp);
                Copy-SingleFile -SourceFileFullPath $InsightFileInfo.FullName -TargetDirectory (Join-Path $ArchiveDirectory $DateTimeStamp);
                $Action = "PS - Content Statictics and article files are archived.";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
            
                #Call bat files
                $Count=0;
                foreach ($BatFile in $OrderedBatFilesToRun){
                    $Count++;
                    $BatOutput = "";

                    $Action = " ";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
                    $Action = "PS - E X E C U T I N G   B A T   F I L E  # $($Count): $BatFile";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action);
                    $Action = " ";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);

                    $InvokeBatFileCommand = $BatFile;
                    $BatOutput = Invoke-Expression $InvokeBatFileCommand;

                    $Action = "PS - BAT output: ";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
                    $Output.Action += $BatOutput;
                    [ASA.Task.Contracts.LogHelper]::WriteLineInfo($BatOutput,$false);

                    if ($LASTEXITCODE -ne 0){throw "BAT file $($BatFile) returned a non-zero exit code of $LASTEXITCODE implying error!";}
                
                }
            }
        }
        catch {
            $Output.Error = $Error[0].Exception.Message;
            [ASA.Task.Contracts.LogHelper]::WriteLineError($Output.Error)

            $Action = " ";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
            $Action = "PS - H A N D L I N G   E R R O R:";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action);
            $Action = " ";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);

            #Move all files to Error directory
            XCopy-Item -CopyFromDir $ArticleFileDirectory -CopyToDir (Join-Path $ErrorDirectory $DateTimeStamp) -DeleteSourceItem;
            Copy-SingleFile -SourceFileFullPath $InsightFileInfo.FullName -TargetDirectory (Join-Path $ErrorDirectory $DateTimeStamp) -DeleteSourceIfCopiedToTarget $true;

            $Action = "PS - All Content Statictics and article files moved to Error directory.";
            [ASA.Task.Contracts.LogHelper]::WriteLineError($Action,$false)
            $Output.Action += $Action;}
        finally 
        {
            $ErrorActionPreference = "Continue";

            $Action = " ";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
            $Action = "PS - F I N A L I Z I N G:";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action);
            $Action = " ";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);

            #Delete files if no error (error would have already moved files to Error directory)
            if ($Output.Error -eq "NONE"){
                [System.IO.File]::Delete($InsightFileInfo.FullName);
                [System.IO.Directory]::GetFiles($ArticleFileDirectory) | 
                %{
                    [System.IO.File]::Delete($_);
                }
                $Action = "PS - All Content Statictics and article files deleted from current session on Endeca server. `r`n";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
            }

            if ($Output.Error -ne "NONE"){
                $Action = "PS - Terminating due to error!";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
                $CombinedEmailTo = @();
                $EmailToSuccessAndFailure | %{if (![string]::IsNullOrEmpty($_)){$CombinedEmailTo += $_}} 
                $EmailToFailureOnly |  %{if (![string]::IsNullOrEmpty($_)){$CombinedEmailTo += $_}} 
                if ($CombinedEmailTo.Count -gt 0){
                    $Action = "PS - Sending email detailing error!";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
                    Send-Email -Environment $Environment -EmailTo $CombinedEmailTo -Subject "ERROR: $EmailSubject" -Body (($Output.Error | Format-List | Out-String) + ($Output | %{$_.Action;} | Format-List | Out-String))
                }
                else {Write-Output New-Object -TypeName PSObject -Property $Output;}
                throw "Terminating due to error!"
            }
        }
    }
    else {$Action = "PS - No action performed since Content Statictics file and at least one article file need to be present and they were not!";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);}

    if ($EmailToSuccessAndFailure.Count -gt 0)
    {
        if ($ContinueArchiveAndProcessBATFiles){
            $Action = "PS - Sending email reporting success!";$Output.Action += $Action;[ASA.Task.Contracts.LogHelper]::WriteLineInfo($Action,$false);
            Send-Email -Environment $Environment -EmailTo $EmailToSuccessAndFailure -Subject "SUCCESS: $EmailSubject" -Body ($Output | %{$_.Action;} | Format-List | Out-String);}
        else {Write-Output New-Object -TypeName PSObject -Property $Output;}
    }
    else {Write-Output New-Object -TypeName PSObject -Property $Output;}
}
end{}


