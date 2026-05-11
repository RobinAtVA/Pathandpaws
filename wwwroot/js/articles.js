function loadPosts(){
let posts=JSON.parse(localStorage.getItem('posts')||'[]');
let el=document.getElementById('posts');el.innerHTML='';
posts.forEach(p=>{let d=document.createElement('div');d.innerHTML=`<h3>${p.title}</h3><p>${p.content}</p>`;el.appendChild(d);});
}
function addPost(){
let t=document.getElementById('title').value;
let c=document.getElementById('content').value;
let posts=JSON.parse(localStorage.getItem('posts')||'[]');
posts.push({title:t,content:c});
localStorage.setItem('posts',JSON.stringify(posts));
loadPosts();
}
window.onload=loadPosts;