function login(url)
{
	var data = {};
	var formData = new FormData();
	formData.append("nim", parseInt(document.getElementsByName("nim")[0].value));
	formData.append("name", document.getElementsByName("name")[0].value);
	formData.append("email", document.getElementsByName("email")[0].value);
	formData.append("password", document.getElementsByName("password")[0].value);

	for(let [key, value] of formData)
	{
		data[key] = value;
	}

	let jsonData = JSON.stringify(data);

	var request = new XMLHttpRequest();

	request.onreadystatechange = function() {
		if(this.readyState == 4 && this.status == 200) {
			res = JSON.parse(this.responseText);
			localStorage.setItem("access_token", res.access_token);
			localStorage.setItem("exp", res.exp);
		}
	}
	request.open('POST', url, true);
	request.setRequestHeader("Content-Type", "application/json");
	request.send(jsonData);
}

function register(url)
{
	var data = {};
	var formData = new FormData();
	formData.append("nim", parseInt(document.getElementsByName("nim")[0].value));
	formData.append("name", document.getElementsByName("name")[0].value);
	formData.append("email", document.getElementsByName("email")[0].value);
	formData.append("password", document.getElementsByName("password")[0].value);
	formData.append("role", document.getElementsByName("role")[0].value);

	for (let [key, value] of formData) {
		data[key] = value;
	}

	let jsonData = JSON.stringify(data);

	let request = new XMLHttpRequest();

	request.open('POST', url, true);
	request.setRequestHeader("Content-Type", "application/json");
	request.send(jsonData);
}

function getPDF()
{
	if (localStorage.getItem("access_token") && !isJwtExpired()) {
		let request = new XMLHttpRequest();
	
		request.onreadystatechange = function() {
			if(this.readyState == 4 && this.status == 200) {
				res = JSON.parse(this.responseText);
				for(let i = 0; i < res.file.length; i++)
				{
					let fileName = res.file[i].split('\\').pop().split('/').pop();
					document.getElementsByTagName("ul")[0].insertAdjacentHTML('beforeend', `<li><a href="${res.file[i]}">${fileName}</a></li>`);
				}
			}
		}
		
		request.open("GET", "https://localhost:5001/Admin/Get", true);
		request.setRequestHeader("Authorization", `Bearer ${localStorage.getItem("access_token")}`);
		request.send();
	}
	else
	{
		window.location.replace("http://127.0.0.1:5500/frontend/login.html");
	}
}

function postPDF()
{
	let formData = new FormData();

	formData.append("File", document.getElementsByName("file")[0].files[0], document.getElementsByName("file")[0].files[0].name);
	let request = new XMLHttpRequest();

	request.open("POST", "https://localhost:5001/Admin/Post", true);
	request.onreadystatechange = function () {
		if (this.readyState == 4 && this.status == 200) {
			console.log(this.responseText)
		}
	}
	request.setRequestHeader("Authorization", `Bearer ${localStorage.getItem("access_token")}`);
	request.send(formData);

}

function isJwtExpired()
{
	let dateNow = new Date().toUTCString();
	let expDate = new Date(localStorage.getItem("exp")).toUTCString();

	if(dateNow.valueOf() > expDate.valueOf())
	{
		return false;
	}

	return true;
}


