<template>
  <b-card class="m-2">
    <b-row align-h="end" class="mb-3 mr-1">
      <b-form-input
        size="lg"
        style="width: 350px"
        v-model="searchValue"
        type="search"
        placeholder="Buscar detalle vale..."
      ></b-form-input>
      <b-button
        variant="primary"
        style="
          background-color: rgb(94,80,238);
          height: 50px;
          width: auto;
          font-size: 18px;
          margin-right: 15px;
          margin-left: 20px;
        "
        @click="showModal = !showModal"
        type="submit"
      >
        <i class="bi bi-person-plus-fill"></i>
        Detalle vale
      </b-button>
    </b-row>
    <EasyDataTable
      rows-per-page-message="registros por pagina"
      empty-message="No se encontro ningun registro"
      table-class-name="customize-table"
      buttons-pagination
      border-cell
      :loading="isloading"
      :headers="fields"
      :items="detailVoucher"
      :rows-per-page="5"
      :search-field="searchField"
      :search-value="searchValue"
    >
      <template #header-actions="header">
        {{ header.text }}
      </template>
      <template #item-actions="items">
        <b-button
          @click="RemoveDetailVoucher(items.detalleValeId)"
          class="m-1"
          variant="outline-danger"
          ><i class="bi bi-trash3"></i
        ></b-button>
        <b-button
          class="m-1"
          variant="outline-warning"
          :to="{
            name: 'DetalleVale-Edit',
            params: { DetalleValeId: items.detalleValeId }
          }"
          ><i class="bi bi-pencil-square"></i
        ></b-button>
      </template>
    </EasyDataTable>
    <b-modal
      id="modal-detailVoucher"
      title="Agregar detalle vale"
      v-model="showModal"
      size="xl"
      hide-footer
      button-size="lg"
      lazy
    >
      <Form @submit="addDetailVoucher">
        <b-row cols="2">
          <!--Agregar cantidad -->
          <b-col>
            <b-form-group class="mt-3" label="Cantidad">
              <Field name="QuantityField" :rules="validateQuantity" as="number">
                <b-form-input
                  v-model="detailVoucherFields.cantidad"
                  :state="QuantityState"
                  type="number"
                >
                </b-form-input>
              </Field>
              <ErrorMessage
                class="text-danger"
                name="QuantityField"
              ></ErrorMessage>
            </b-form-group>
          </b-col>
          <!--Agregar Precio -->
          <b-col>
            <b-form-group class="mt-3" label="Precio">
              <Field name="PriceField" :rules="validatePrice" as="number">
                <b-form-input
                  v-model="detailVoucherFields.precio"
                  :state="PriceState"
                  type="number"
                >
                </b-form-input>
              </Field>
              <ErrorMessage
                class="text-danger"
                name="PriceField"
              ></ErrorMessage>
            </b-form-group>
          </b-col>
          <!--Agregar subtotal -->
          <b-col>
            <b-form-group class="mt-3" label="Subtotal">
              <Field name="TotalField" :rules="validateTotal" as="number">
                <b-form-input
                  v-model="detailVoucherFields.subtotal"
                  :state="TotalState"
                  type="number"
                >
                </b-form-input>
              </Field>
              <ErrorMessage
                class="text-danger"
                name="TotalField"
              ></ErrorMessage>
            </b-form-group>
          </b-col>
          <!--Agregar Producto-->
          <b-col>
            <b-form-group class="mt-3" label="Producto: ">
              <Field
                name="ProductField"
                :rules="validateProduct"
                as="text"
              >
                <b-form-select
                  v-model="detailVoucherFields.productoId"
                  autofocus
                  :options=""
                  value-field="productoId"
                  text-field="nombre"
                  :state="ProductState"
                >
                </b-form-select>
              </Field>
              <ErrorMessage
                class="text-danger"
                name="ProductField"
              ></ErrorMessage>
            </b-form-group>
          </b-col>
          <!--termino de cada campo a registrar-->
        </b-row>
        <b-row align-h="end">
          <b-button
            class="w-auto m-2 text-white"
            variant="primary"
            @click="resetDetailVoucherFields"
          >
            Cancelar
          </b-button>
          <b-button class="w-auto m-2" variant="success" type="submit">
            Guardar
          </b-button>
        </b-row>
      </Form>
    </b-modal>
  </b-card>
</template>

<script>
import DetailVoucherServices from '@/Services/detailvoucher.Services'
import { Form, Field, ErrorMessage } from 'vee-validate'
import { ref, inject } from 'vue'
import '@vuepic/vue-datepicker/dist/main.css'
export default {
  components: {
    EasyDataTable: window['vue3-easy-data-table'],
    Form,
    Field,
    ErrorMessage
  },
  setup () {
    const swal = inject('$swal')
    const showModal = ref(false)
    const { getDetailVoucher, createDetailVoucher, deleteDetailVoucher } = DetailVoucherServices()
    const detailVoucher = ref([])
    const perPage = ref(5)
    const currentPage = ref(1)
    const filter = ref(null)
    const perPageSelect = ref([5, 10, 25, 50, 100])
    const isloading = ref(true)
    const searchValue = ref('')
    const searchField = ref('agregardato para busqueda')
    const QuantityState = ref(false)
    const PriceState = ref(false)
    const TotalState = ref(false)
    const ProductState = ref(false)
    const detailVoucherFields = ref({
      detalleValeId: 0,
      cantidad: null,
      precio: null,
      subtotal: null,
      productoId: null
      archivado: false
    })

    const detailVoucherFieldsBlank = ref(
      JSON.parse(JSON.stringify(detailVoucherFields))
    )
    const fields = ref([
      { value: 'detalleValeId', text: 'ID', sortable: true },
      { value: 'cantidad', text: 'Cantidad' },
      { value: 'precio', text: 'Precio' },
      { value: 'subtotal', text: 'Subtotal' },
      { value: 'proV_Producto.nombre', text: 'Nombre del producto' },
      { value: 'actions', text: 'Acciones' }
    ])

    const resetDetailVoucherFields = () => {
      showModal.value = false
      detailVoucherFields.value = JSON.parse(
        JSON.stringify(detailVoucherFieldsBlank)
      )
      QuantityState.value = false
      PriceState.value = false
      TotalState.valu = false
      ProductState.value = false
    }

    getDetailVoucher(data => {
      detailVoucher.value = data
      if (detailVoucher.value.length > 0) {
        isloading.value = false
      } else {
        if (detailVoucher.value.length <= 0) {
          isloading.value = false
        }
      }
    })

    const onFiltered = filteredItems => {
      currentPage.value = 1
    }

    const validateQuantity = () => {
      if (!detailVoucher.value.cantidad) {
        QuantityState.value = false
        return 'Este campo es requerido'
      }

      if (!/^[0-9]+$/i.test(detailVoucher.value.cantidad)) {
        QuantityState.value = false
        return 'Este campo solo puede contener numeros'
      }
      QuantityState.value = false
      return true
    }

    const validatePrice = () => {
      if (!detailVoucher.value.precio) {
        PriceState.value = false
        return 'Este campo es requerido'
      }

      if (!/^[0-9]+$/i.test(detailVoucher.value.precio)) {
        PriceState.value = false
        return 'Este campo solo puede contener numeros'
      }
      PriceState.value = false
      return true
    }

    const validateTotal = () => {
      if (!detailVoucher.value.subtotal) {
        TotalState.value = false
        return 'Este campo es requerido'
      }

      if (!/^[0-9]+$/i.test(detailVoucher.value.subtotal)) {
        TotalState.value = false
        return 'Este campo solo puede contener numeros'
      }
      TotalState.value = false
      return true
    }

    const validateProduct = () => {
      if (!voucherControlFields.value.productoId) {
        ProductState.value = false
        return 'Este campo es requerido'
      }
      ProductState.value = false
      return true
    }

    // pone mis cambios de mis campos vacios de nuevo
    const refreshTable = () => {
      isloading.value = true
      getDetailVoucher(data => {
        detailVoucher.value = data
        if (detailVoucher.value.length > 0) {
          isloading.value = false
        } else {
          if (detailVoucher.value.length <= 0) {
            isloading.value = false
          }
        }
      })
      return 'datos recargados'
    }
    const addDetailVoucher = () => {
      createDetailVoucher(detailVoucherFields.value, data => {
        refreshTable()
        swal.fire({
          title: '??Detalle vale registrado correctamente!',
          text: 'El detalle vale se ha registrado al sistema satisfactoriamente.',
          icon: 'success'
        })
      })
      showModal.value = false
      resetDetailVoucherFields()
    }
    const RemoveDetailVoucher = WorksStatusId => {
      isloading.value = true
      swal.fire({
        title: '??Estas seguro?',
        text: 'No podr??s revertir esto!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Si, Archivar detalle vale!',
        cancelButtonText: 'Cancelar'
      })
        .then(result => {
          if (result.isConfirmed) {
            deleteDetailVoucher(WorksStatusId, (data) => {
              refreshTable()
            })
            swal
              .fire({
                title: '??Detalle vale archivado!',
                text: 'El detalle vale ha sido archivado satisfactoriamente .',
                icon: 'success'
              })
          } else {
            isloading.value = false
          }
        })
    }
    return {
      detailVoucher,
      detailVoucherFields,
      showModal,
      perPage,
      currentPage,
      filter,
      perPageSelect,
      isloading,
      searchValue,
      searchField,
      detailVoucherFieldsBlank,
      fields,
      DetailVoucherState,
      ProductState,
      onFiltered,
      addDetailVoucher,
      refreshTable,
      RemoveDetailVoucher,
      validateQuantity,
      validatePrice,
      validateTotal,
      validateProduct,
      resetDetailVoucherFields
    }
  }
}
</script>

<style>

</style>
