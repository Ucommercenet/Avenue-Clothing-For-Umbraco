$(function () {
    $('#site-search').typeahead({
        minLength: 3,
        source: function (query, process) {
            $.uCommerce.search({ keyword: query }, function (resp) {
                $.map(resp, function (data) {
                    var matches = [];
                    for (var i = 0; i < data.length; i++) {
                        matches.push(data[0].ProductName);
                    }
                    return process(matches);
                });
            });
        }
    });
});
