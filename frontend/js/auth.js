function logFormData()
{
	var data = {};
	var formData = new FormData();
	formData.append("nim", document.getElementsByName("nim")[0].value);
	formData.append("name", document.getElementsByName("name")[0].value);
	formData.append("email", document.getElementsByName("email")[0].value);
	formData.append("password", document.getElementsByName("password")[0].value);

	for(let [key, value] of formData)
	{
		data[key] = value;
		console.log(key + ":" + value);
	}

	let jsonData = JSON.stringify(data);

	console.log(jsonData);

	// var request = new XMLHttpRequest();
	// request.open('POST', 'https://localhost:5001/Auth/Login', true);
	// request.send(data);
}

