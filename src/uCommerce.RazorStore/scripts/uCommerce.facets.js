$(function () {
	ensureCheckboxesAreChecked();
	$('.filter').click(createQueryString);
});

function createQueryString() {
	var queryStrings = {};
	var baseUrl = window.location.href.split('?')[0]+'?';
	var allChecked = $('.filter:checked');
	allChecked.each(function () {
		var key = $(this).attr('key');
		if (queryStrings[key] == null) {
			queryStrings[key] = $(this).attr('value').toString() + '|';
		} else {
			queryStrings[key] += $(this).attr('value').toString() + '|';
		}
	});

	for (var propertyName in queryStrings) {
		baseUrl += propertyName + '=' + queryStrings[propertyName] + '&';
	}
	var newUrl = baseUrl.substring(0, baseUrl.length - 1);
	window.location.href = newUrl;
}

function ensureCheckboxesAreChecked() {
	var result = {}, queryString = location.search.slice(1),
        re = /([^&=]+)=([^&]*)/g, m;

	while (m = re.exec(queryString)) {
		result[decodeURIComponent(m[1])] = decodeURIComponent(m[2]);
	}

	var params = result;

	for (var propertyName in params) {
		var value = params[propertyName].split('|');

		for (var i = 0; i < value.length - 1; i++) {
			var filter = '.filter[key="' + propertyName + '"][value="' + value[i] + '"]';
			var checkbox = $(filter);
			if (checkbox != null) {
				checkbox.attr('checked', 'checked');
			}
		}
	}
}