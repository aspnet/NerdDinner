/// <binding Clean='clean' />

var gulp = require("gulp"),
  rimraf = require("rimraf"),
  fs = require("fs"),
  less = require('gulp-less'),
  uglify = require('gulp-uglifyjs');

eval("var project = " + fs.readFileSync("./project.json"));

var paths = {
  bower: "./bower_components/",
  lib: "./" + project.webroot + "/lib/",
  views: "./" + project.webroot + "/views/",
  styles: "./" + project.webroot + "/css/",
  images: "./" + project.webroot + "/images/",
};

gulp.task("clean", function (cb) {
  rimraf(paths.lib, cb);
});

gulp.task("copy", ["clean"], function () {
  var bower = {
    "bootstrap": "bootstrap/dist/**/*.{js,map,css,ttf,svg,woff,eot}",
    "bootstrap-touch-carousel": "bootstrap-touch-carousel/dist/**/*.{js,css}",
    "hammer.js": "hammer.js/hammer*.{js,map}",
    "jquery": "jquery/jquery*.{js,map}",
    "jquery-validation": "jquery-validation/jquery.validate.js",
    "jquery-validation-unobtrusive": "jquery-validation-unobtrusive/jquery.validate.unobtrusive.js",
    "angular": "angular/angular*.{js,map}",
    "angular-bootstrap": "angular-bootstrap/ui-bootstrap*.js",
    "angular-resource": "angular-resource/angular-resource*.{js,map}",
    "angular-route": "angular-route/angular-route*.js"
  }

  for (var destinationDir in bower) {
    gulp.src(paths.bower + bower[destinationDir])
      .pipe(gulp.dest(paths.lib + destinationDir));
  }

  gulp.src('ng-apps/views/*.html')
    .pipe(gulp.dest(paths.views));

  gulp.src('ng-apps/content/images/*.*')
    .pipe(gulp.dest(paths.images));
});

gulp.task('less', function () {
    return gulp.src('ng-apps/content/styles/*.less')
      .pipe(less())
      .pipe(gulp.dest(paths.styles));
});

gulp.task('uglify', function () {
    return gulp.src('ng-apps/**/*.js')
      .pipe(uglify('app.js', {
          mangle: false,
          output: {
              beautify: true
          }
      }))
      .pipe(gulp.dest(project.webroot));
});