$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    $('#tblData').DataTable({
        "ajax": { url: '/company/getall' },
        "columns": [
            { data: 'name', "width": "25%" },
            { data: 'streetAddress', "width": "15%" },
            { data: 'city', "width": "10%" },
            { data: 'state', "width": "15%" },
            { data: 'phoneNumber', "width": "10%" },
            {
                data: 'id',
                'render': function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/company/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>
                        <button class="btn btn-danger mx-2" onclick="deleteCompany(${data})"> <i class="bi bi-trash3-fill"></i> Delete</button>
                    </div>`;
                },
                "width": "25%"
            }
        ]
    });
}

function deleteCompany(id) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: `/company/Delete?id=${id}`,
                type: 'DELETE',
                success: function (result) {
                    if (result.success) {
                        Swal.fire({
                            title: "Deleted!",
                            text: result.message,
                            icon: "success"
                        }).then(() => {
                            $('#tblData').DataTable().ajax.reload(); // Refresh the DataTable
                        });
                    } else {
                        Swal.fire({
                            title: "Error!",
                            text: result.message,
                            icon: "error"
                        });
                    }
                },
                error: function (xhr, status, error) {
                    Swal.fire({
                        title: "Error!",
                        text: "An error occurred: " + error,
                        icon: "error"
                    });
                }
            });
        }
    });
}
