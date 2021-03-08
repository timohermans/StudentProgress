const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');

module.exports = {
  entry: {
    site: './src/js/site.js',
    groupDetails: './src/js/group-details/index.js',
    studentSummary: './src/js/student-summary/index.js',
    progress: './src/js/progress/index.js'
  },
  output: {
    filename: '[name].entry.js',
    path: path.resolve(__dirname, '..', 'wwwroot', 'dist')
  },
  devtool: 'source-map',
  mode: 'development',
  module: {
    rules: [
      { test: /\.css$/, use: [{ loader: MiniCssExtractPlugin.loader }, "css-loader"] },
      { test: /\.eot(\?v=\d+\.\d+\.\d+)?$/, use: "file-loader" },
      { test: /\.(woff|woff2)$/, use: "url-loader?prefix=font/&limit=5000" },
      { test: /\.ttf(\?v=\d+\.\d+\.\d+)?$/, use: "url-loader?limit=10000&mimetype=application/octet-stream" },
      { test: /\.svg(\?v=\d+\.\d+\.\d+)?$/, use: "url-loader?limit=10000&mimetype=image/svg+xml" }
    ]
  },
  plugins: [
    new MiniCssExtractPlugin({
      filename: "[name].css"
    })
  ]
};