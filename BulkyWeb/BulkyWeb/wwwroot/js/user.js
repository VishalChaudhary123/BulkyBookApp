$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    $('#tblData').DataTable({
        "ajax": { url: '/user/getall' },
        "columns": [
            { data: 'name', "width": "25%" },
            { data: 'email', "width": "15%" },
            { data: 'phoneNumber', "width": "10%" },
            { data: 'company.name', "width": "15%" },
            { data: 'role', "width": "10%" },
            {
                data: {id:'id', lockoutEnd: 'lockoutEnd'},
                'render': function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockout).getTime();

                    if (data.lockout > today) {
                        return `
                        <div class="text-center">
                       
                        <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:100px;"> <i class="bi bi-lock-fill"></i> Lock
                        </a>
                         <a  class="btn btn-danger text-white" style="cursor:pointer; width:150px;"> <i class="bi bi-pencil-square"></i> Permission
                        </a>
                      
                    </div>`;
                    }
                    else {
                        return `
                        <div class="text-center">
                         <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer; width:100px;"> <i class="bi bi-unlock-fill"></i> Unlock
                        </a>
                         <a  class="btn btn-danger text-white" style="cursor:pointer; width:150px;"> <i class="bi bi-pencil-square"></i> Permission
                        </a>
                      
                    </div>`;
                    }
                    //return `<div class="w-75 btn-group" role="group">
                    //    <a href="/company/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>
                      
                    //</div>`;
                },
                "width": "25%"
            }
        ]
    });
}

function LockUnlock(id) {
    $.ajax({
        type: "POST",   
        url: 'User/LockUnlock',
        data: JSON.stringify({ id: id }), // Ensure id is wrapped in an object
        contentType: "application/json",
        success: function (data) { // Fixed typo here
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        },
        error: function (xhr, status, error) {
            toastr.error("An error occurred: " + error); // Optional error handling
        }
    });
}
