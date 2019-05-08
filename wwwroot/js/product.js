$(function () {
    getProducts();

    function getProducts() {
        var id = $('#product_rows').data('id');
        var filter = "";
        var radios = document.getElementsByName('productFilter');
        var url = "";

        for (var i = 0; i < radios.length; i++) {
            if (radios[i].checked) {
                if (radios[i].value == 'Discontinued') {
                    filter = '/Discontinued';
                    break;
                } else if (radios[i].value == 'Reorder') {
                    filter = '/Reorder';
                    break;
                } else if (radios[i].value == 'OutOfStock') {
                    filter = '/OutOfStock';
                    break;
                }                
            }
        }
        if (id == 0) {
            url = "../../api";
        } else {
            url = "../../api/category/" + id;
        }

        $.getJSON({
            url: url + "/product" + filter,
            success: function (response, textStatus, jqXhr) {
                //console.log(response);
                $('#product_rows').html("");
                for (var i = 0; i < response.length; i++){
                    var css = response[i].discontinued ? " class=\"discontinued\"" : "";
                    var row = "<tr" + css + " data-id=\"" + response[i].productId + "\" data-name=\"" + response[i].productName + "\" data-price=\"" + response[i].unitPrice + "\">"
                        + "<td>" + response[i].productName + "</td>"
                        + "<td class=\"text-right\">$" + response[i].unitPrice.toFixed(2) + "</td>"
                        + "<td class=\"text-right\">" + response[i].unitsInStock + "</td>"
                        + "</tr>";
                    $('#product_rows').append(row);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                // log the error to the console
                console.log("The following error occured: " + textStatus, errorThrown);
            }
        });
    }

    getProducts();

    $('#CategoryId').on('change', function(){
        $('#product_rows').data('id', $(this).val());
        getProducts();
    });

    $('.productFilter').on('change', function () {
        getProducts();
    });

    // delegated event listener
    $('#product_rows').on('click', 'tr', function(){
        // make sure a customer is logged in
        if ($('#User').data('customer').toLowerCase() == "true"){
            $('#ProductId').html($(this).data('id'));
            $('#ProductName').html($(this).data('name'));
            $('#UnitPrice').html($(this).data('price').toFixed(2));
            // calculate and display total in modal
            $('#Quantity').change();
            $('#cartModal').modal();
        } else {
            toast("Access Denied", "You must be signed in as a customer to access the cart.");
        }

    });

    // update total when cart quantity is changed
    $('#Quantity').change(function () {
        var total = parseInt($(this).val()) * parseFloat($('#UnitPrice').html());
        $('#Total').html(numberWithCommas(total.toFixed(2)));
    });

    // function to display commas in number
    function numberWithCommas(x) {
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }

    $('#addToCart').on('click', function(){
        $('#cartModal').modal('hide');
        // AJAX to update database
        $.ajax({
            headers: { "Content-Type": "application/json" },
            url: "../../api/addtocart",
            type: 'post',
            data: JSON.stringify({
                    "id": $('#ProductId').html(),
                    "email": $('#User').data('email'),
                    "qty": $('#Quantity').val() 
                }),
            success: function (response, textStatus, jqXhr) {
                // success
                toast("Product Added", response.product.productName + " successfully added to cart.");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                // log the error to the console
                toast("Error", "Please try again later.");
                console.log("The following error occured: " + jqXHR.status, errorThrown);
            }
        });
    });

    function toast(header, message) {
        $('#toast_header').html(header);
        $('#toast_body').html(message);
        $('#cart_toast').toast({ delay: 2500 }).toast('show');
    }
});
