param($installPath, $toolsPath, $package, $project)

cp $toolsPath\gulpfile.js $installPath\..\..\gulpfile.js
$npmPath = (join-path $toolsPath "npm.cmd")
& $npmPath install gulp -g
& $npmPath install gulp gulp-util --save-dev
