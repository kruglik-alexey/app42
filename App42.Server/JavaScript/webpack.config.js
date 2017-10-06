const webpack = require('webpack');

module.exports = env => {
    const isProduction = env === "production";
    var config = {
        entry: './entry.js',
        output: {
            path: __dirname + '/../wwwroot',
            filename: 'app.js'
        },
        module: {
            rules: [
                {
                    test: /\.jsx?$/,
                    exclude: /(node_modules)/,
                    use: {
                        loader: 'babel-loader',
                        options: {
                            presets: [
                                [
                                    "env", {
                                        "targets": {
                                            "browsers": ["last 2 versions"]
                                        },
                                        useBuiltIns: true
                                    }
                                ],
                                ["react"]
                            ]
                        }
                    }
                },
                {
                    test: /\.css$/,
                    use: [
                        {loader: 'style-loader'},
                        {loader: 'css-loader'}
                    ]
                }
            ]
        },
        plugins: [],
        devServer: {
            port: 9000,
            compress: true,
            noInfo: true
        }
    };

    if (isProduction) {        
        config.plugins.push(new webpack.DefinePlugin({
            'process.env': {
                NODE_ENV: JSON.stringify('production')
            }
        }));
        config.plugins.push(new webpack.optimize.UglifyJsPlugin());
    }

    return config;
};
