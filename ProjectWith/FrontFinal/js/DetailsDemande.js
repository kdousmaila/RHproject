@section scripts{
    <script>


        $("#RefuserDemande").on("click", function () {
            $('#ModalRefus').modal('show');

        });

        $("#AcccepterDemande").on("click", function () {
            $('#ModalAccepte').modal('show');


        });

        // $("#AnnulerSaveRefus").on("click", function () {
            //     location.href = "@Url.Action("Details", "Demandes", new { id = @Model.Id })";
            // });

            $("#AnnulerSaveRefus").on("click", function () {
                var id = @Model.Id;
                // location.href = `/Demandes/Details/${id}`;
                location.href = "@Url.Action("Details", "Demandes", new { id = @Model.Id })";
            });

        $("#Annuleraccc").on("click", function () {
            var id = @Model.Id;
        location.href = `/Demandes/Details/${id}`;
        });

        $("#SaveRefus").on("click", function () {

            var iddemande = $("#Id").val();



        let button = document.querySelector("#SaveRefus");
        if ($("#cause").val() == '') {

            required: true

            }
        else {
            button.disabled = false;



        $.ajax({
            dataType: "json",
        type: "POST",
        url: "@Url.Action("RefuserDemande", "Demandes")",
        data: {iddemande},
        success: function (data) {
            console.log("data", data);

        Swal.fire({

            icon: 'success',
        title: 'Demande Refuser',
        text: 'Demande Refuser!!',

                    }).then((result) => {
            location.href = "@Url.Action("Details", "Demandes", new {id = @Model.Id })";
                    })

                }

            });

            }

        });

        $("#SaveAccepete").on("click", function () {

            var iddemande = $("#Id").val();

        $.ajax({
            dataType: "json",
        type: "POST",
        url: "@Url.Action("AccepterDemande", "Demandes")",
        data: {iddemande: iddemande },

        success: function (data) {
            alert(data);  // Afficher les données dans la console
        $('#ModalAccepte').modal('hide');
        location.href = "/Demandes/Details/" + @Model.Id;

        Swal.fire({
            icon: 'success',
        title: 'Demande Acceptée',
        text: 'Demande Acceptée !!',
                    }).then((result) => {
            location.href = "/Demandes/Details/" + @Model.Id;
                    });
                },
        error: function (xhr, status, error) {
            console.error("Erreur Ajax:", status, error);
        // Réactiver le bouton en cas d'erreur
        $("#SaveAccepete").prop("disabled", false);
                }
            });

        });

    </script>

}