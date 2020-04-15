// <binding Clean='clean' AfterBuild='postbuild'/>

var gulp  = require('gulp'),
    gutil = require('gulp-util');
var sass  = require('gulp-sass');
var watch = require('gulp-watch');
var notify = require('gulp-notify');


gulp.task('hello-world', function(){
    console.log('My first gulp task');
});

//SASS
gulp.task('sass', function() {
    gulp.src('AvenueClothing/css/*.scss')
        .pipe(sass().on('error', sass.logError))
        .pipe(gulp.dest('AvenueClothing/css/'))
        .pipe(notify({ message: 'Sass has been processed.' }));
});

gulp.task('watch-sass', function() {
    gulp.watch('AvenueClothing/css/*.scss', ['sass']);  // If a file changes, re-run 'sass'
});
