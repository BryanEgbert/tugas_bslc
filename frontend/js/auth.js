function login(url)
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
	}

	let jsonData = JSON.stringify(data);

	var request = new XMLHttpRequest();

	request.onreadystatechange = function() {
		if(this.readyState == 4 && this.status == 200) {
			res = JSON.parse(this.responseText);
			localStorage.setItem("access_token", res.access_token);
			localStorage.setItem("exp", res.exp);
			localStorage.setItem("email", document.getElementsByName("email")[0].value);
		}
	}
	request.open('POST', url, true);
	request.setRequestHeader("Content-Type", "application/json");
	request.send(jsonData);

	document.getElementById("loginForm").reset();
}

function register(url)
{
	var data = {};
	var selectTagValue = document.getElementById("role");
	var formData = new FormData();
	formData.append("nim", document.getElementsByName("nim")[1].value);
	formData.append("name", document.getElementsByName("name")[1].value);
	formData.append("email", document.getElementsByName("email")[1].value);
	formData.append("password", document.getElementsByName("password")[1].value);
	formData.append("role", selectTagValue.options[selectTagValue.selectedIndex].text);

	for (let [key, value] of formData) {
		data[key] = value;
	}

	let jsonData = JSON.stringify(data);

	let request = new XMLHttpRequest();

	request.open('POST', url, true);
	request.setRequestHeader("Content-Type", "application/json");
	request.send(jsonData);

	doccument.getElementById("registerForm").reset();
}

function getUserRole()
{
	let request = new XMLHttpRequest();
	request.onreadystatechange = function()
	{
		if(this.readyState == 4 && this.status == 200)
		{
			res = JSON.parse(this.responseText);
			if(res[0] == "Admin")
			{
				let nav = document.getElementsByClassName("link")[0];
				nav.insertAdjacentHTML("beforeend", '<a href="admin.html">Admin</a>')
			}
		}
	}
	request.open("POST", "https://localhost:5001/Users/User/GetUserRole", true);
	request.send(localStorage.getItem("email"));
}

function getPDFAdmin() {
	if (localStorage.getItem("access_token") && isJwtExpired() == false) {
		let request = new XMLHttpRequest();

		request.onreadystatechange = function () {
			if (this.readyState == 4 && this.status == 200) {
				res = JSON.parse(this.responseText);
				for (let i = 0; i < res.file.length; i++) {
					let fileName = res.file[i].split('\\').pop().split('/').pop();
					document.getElementsByTagName("ul")[0].insertAdjacentHTML('beforeend', `<li><a href=""http://127.0.0.1:5500/backend/File/${fileName}">${fileName}</a> <button type="button" id="${i}" onclick="deletePDF(${i})">delete</button></li>`);
				}
			}
		}

		request.open("GET", "https://localhost:5001/Docs/Get", true);
		request.setRequestHeader("Authorization", `Bearer ${localStorage.getItem("access_token")}`);
		request.send();
	}
	else {
		window.location.replace("http://127.0.0.1:5500/frontend/login.html");
	}
}

function getPDF()
{
	if (localStorage.getItem("access_token") && isJwtExpired() == false) {
		let request = new XMLHttpRequest();
	
		request.onreadystatechange = function() {
			if(this.readyState == 4 && this.status == 200) {
				res = JSON.parse(this.responseText);
				for(let i = 0; i < res.file.length; i++)
				{
					let fileName = res.file[i].split('\\').pop().split('/').pop();
					document.getElementsByTagName("ul")[0].insertAdjacentHTML('beforeend', `<li><a href="http://127.0.0.1:5500/backend/File/${fileName}">${fileName}</a></li>`);
				}
			}
		}
		
		request.open("GET", "https://localhost:5001/Docs/Get", true);
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
	request.open("POST", "https://localhost:5001/Docs/Post", true);
	request.onreadystatechange = function () {
		if (this.readyState == 4 && this.status == 200) {
			console.log(this.responseText)
		}
	}
	request.setRequestHeader("Authorization", `Bearer ${localStorage.getItem("access_token")}`);
	request.send(formData);

}

function deletePDF(index)
{
	var data = {};
	data["FileName"] = document.getElementsByTagName("a")[index].getInnerHTML();
	
	let jsonData = JSON.stringify(data);

	let request = new XMLHttpRequest();
	request.open("DELETE", "https://localhost:5001/Docs/Delete", true);
	request.onreadystatechange = function () {
		if (this.readyState == 4 && this.status == 200) {
			console.log(this.responseText)
		}
	}

	request.setRequestHeader("Authorization", `Bearer ${localStorage.getItem("access_token")}`);
	request.setRequestHeader("Content-Type", "application/json");
	request.send(jsonData);
}

function isJwtExpired()
{
	let dateNow = new Date().toUTCString();
	let expDate = new Date(localStorage.getItem("exp")).toUTCString();

	if(dateNow.valueOf() < expDate.valueOf())
	{
		return true;
	}
	else
	{
		return false;
	}

}


