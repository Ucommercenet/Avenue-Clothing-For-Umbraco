// For more information on how to configure a task runner, please visit:
// https://github.com/gulpjs/gulp

var gulp  = require('gulp'),
    gutil = require('gulp-util');

gulp.task('default', function() {
    gutil.log('Gulp.js has been successfully installed!\n' +
              'For more information on how to configure it, please visit:\n' +
              'https://github.com/gulpjs/gulp');
});