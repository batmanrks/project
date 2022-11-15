<template>
  <b-card class="m-2">
    <b-row align-h="end" class="mb-3 mr-1">
      <b-form-input
        size="lg"
        style="width: 350px;"
        v-model="searchValue"
        type="search"
        placeholder="Buscar Vale..."
      ></b-form-input>
      <b-button
        variant="primary"
        style="
          background-color: rgb(94, 80, 238);
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
        Consulta de vales
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
      :items="voucherControl"
      :rows-per-page="5"
      :search-field="searchField"
      :search-value="searchValue"
    >
      <template #header-actions="header">
        {{ header.text }}
      </template>
      <template #item-actions="items">
        <b-button
          @click="RemoveVoucherControl(items.controlValeId)"
          class="m-1"
          variant="outline-danger"
          ><i class="bi bi-trash3"></i
        ></b-button>
        <b-button
          class="m-1"
          variant="outline-warning"
          :to="{
            name: 'ControlVale-Edit',
            params: { ControlValeId: items.controlValeId },
          }"
          ><i class="bi bi-pencil-square"></i
        ></b-button>
      </template>
    </EasyDataTable>
    <b-modal
      id="modal-voucherControl"
      title="Agregar Vale"
      v-model="showModal"
      size="xl"
      hide-footer
      button-size="lg"
      lazy
    >
      <Form @submit="addVoucherControl">
        <b-row cols="2">
          <!--Agregar Fecha emicion-->
          <b-col>
            <b-form-group class="mt-3" label="Fecha emicion">
              <Field name="DateOfIssueField" :rules="validateDateOfIssue" as="">
                <Datepicker
                  locale="es"
                  name="date"
                  text-input
                  v-model="voucherControlFields.fechaEmicion"
                  :state="DateOfIssueState"
                ></Datepicker>
              </Field>
              <ErrorMessage name="DateOfIssueField"></ErrorMessage>
            </b-form-group>
          </b-col>
          <!--Agregar Fecha vigencia-->
          <b-col>
            <b-form-group class="mt-3" label="Fecha vigencia">
              <Field
                name="ExpirationDateField"
                :rules="validateExpirationDate"
                as=""
              >
                <Datepicker
                  locale="es"
                  name="date"
                  text-input
                  v-model="voucherControlFields.fechaVigencia"
                  :state="ExpirationDateState"
                ></Datepicker>
              </Field>
              <ErrorMessage name="ExpirationDateField"></ErrorMessage>
            </b-form-group>
          </b-col>
          <!--Agregar Tipo conbustible-->
          <b-col>
            <b-form-group class="mt-3" label="Tipo de combustible">
              <Field name="FuelTypeField" :rules="validateFuelType" as="text">
                <b-form-input
                  v-model="voucherControlFields.tipoCombustible"
                  :state="FuelTypeState"
                >
                </b-form-input>
              </Field>
              <ErrorMessage
                class="text-danger"
                name="FuelTypeField"
              ></ErrorMessage>
            </b-form-group>
          </b-col>
          <!--Agregar Departamento-->
          <b-col>
            <b-form-group class="mt-3" label="Departamento: ">
              <Field
                name="DepartamentField"
                :rules="validateDepartament"
                as="number"
              >
                <b-form-select
                  v-model="voucherControlFields.departamentoId"
                  autofocus
                  :options="departaments"
                  value-field="departamentoId"
                  text-field="nombre"
                  :state="departamentState"
                  @input="getAreas(EmployeesFields.departamentoId)"
                >
                </b-form-select>
              </Field>
              <ErrorMessage
                class="text-danger"
                name="DepartamentField"
              ></ErrorMessage>
            </b-form-group>
          </b-col>
          <!--Agregar Empleado-->
          <b-col>
            <b-form-group class="mt-3" label="Empleado: ">
              <Field
                name="EmployeeField"
                :rules="validateEmployee"
                as="text"
              >
                <b-form-select
                  v-model="voucherControlFields.empleadoId"
                  autofocus
                  :options=""
                  value-field="empleadoId"
                  text-field="nombreCompleto"
                  :state="EmployeeState"
                >
                </b-form-select>
              </Field>
              <ErrorMessage
                class="text-danger"
                name="EmployeeField"
              ></ErrorMessage>
            </b-form-group>
          </b-col>
          <!--Agregar Proveedor-->
          <b-col>
            <b-form-group class="mt-3" label="Nombre proveedor: ">
              <Field
                name="ProviderField"
                :rules="validateProvider"
                as="text"
              >
                <b-form-select
                  v-model="voucherControlFields.proveedorId"
                  autofocus
                  :options=""
                  value-field="proveedorId"
                  text-field="nombre"
                  :state="ProviderState"
                >
                </b-form-select>
              </Field>
              <ErrorMessage
                class="text-danger"
                name="ProviderField"
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
                  v-model="voucherControlFields.productoId"
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
          <!--Agregar Detalle vale-->
          <b-col>
            <b-form-group class="mt-3" label="Detalle vale: ">
              <Field
                name="DetailVoucherField"
                :rules="validateDetailVoucher"
                as="text"
              >
                <b-form-select
                  v-model="voucherControlFields.detalleValeId"
                  autofocus
                  :options=""
                  value-field="detalleValeId"
                  text-field="cantidad"
                  :state="DetailVoucherState"
                >
                </b-form-select>
              </Field>
              <ErrorMessage
                class="text-danger"
                name="DetailVoucherField"
              ></ErrorMessage>
            </b-form-group>
          </b-col>
          <!--Agregar Estatus vale-->
          <b-col>
            <b-form-group class="mt-3" label="Estatus vale: ">
              <Field
                name="StatusVoucherField"
                :rules="validateStatusVoucher"
                as="text"
              >
                <b-form-select
                  v-model="voucherControlFields.estatusValeId"
                  autofocus
                  :options=""
                  value-field="estatusValeId"
                  text-field="estatusVale"
                  :state="StatusVoucherState"
                >
                </b-form-select>
              </Field>
              <ErrorMessage
                class="text-danger"
                name="StatusVoucherField"
              ></ErrorMessage>
            </b-form-group>
          </b-col>
          <!--Agregar Tipo-->
          <b-col>
            <b-form-group class="mt-3" label="Tipo vale: ">
              <Field
                name="TypeVoucherField"
                :rules="validateTypeVoucher"
                as="text"
              >
                <b-form-select
                  v-model="voucherControlFields.tipoId"
                  autofocus
                  :options=""
                  value-field="tipoId"
                  text-field="nombreVale"
                  :state="TypeVoucherState"
                >
                </b-form-select>
              </Field>
              <ErrorMessage
                class="text-danger"
                name="TypeVoucherField"
              ></ErrorMessage>
            </b-form-group>
          </b-col>
        </b-row>
        <b-row align-h="end">
          <b-button
            class="w-auto m-2 text-white"
            variant="primary"
            @click="resetVoucherControlFields"
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
import VoucherControlServices from "@/Services/vouchercontrol.Services";
import { Form, Field, ErrorMessage } from "vee-validate";
import { ref, inject } from "vue";
import "@vuepic/vue-datepicker/dist/main.css";
export default {
  components: {
    EasyDataTable: window["vue3-easy-data-table"],
    Form,
    Field,
    ErrorMessage,
  },
  setup() {
    const swal = inject("$swal");
    const showModal = ref(false);
    const { getVoucherControl, createVoucherControl, deleteVoucherControl, } = VoucherControlServices();
    const voucherControl = ref([]);
    const perPage = ref(5);
    const currentPage = ref(1);
    const filter = ref(null);
    const perPageSelect = ref([5, 10, 25, 50, 100]);
    const isloading = ref(true);
    const searchValue = ref("");
    const searchField = ref("empleadoId");
    const DateOfIssueState = ref(false);
    const ExpirationDateState = ref(false);
    const FuelTypeState = ref(false)
    const departamentState = ref(false)
    const EmployeeState = ref(false)
    const ProviderState = ref(false)
    const ProductState = ref(false)
    const DetailVoucherState = ref(false)
    const StatusVoucherState = ref(false)
    const TypeVoucherState = ref(false)
    const voucherControlFields = ref({
      controlValeId: 0,
      fechaEmicion: null,
      fechaVigencia: null,
      tipoCombustible: null,
      departamentoId: null,
      empleadoId: null,
      proveedorId: null,
      productoId: null,
      detalleValeId: null,
      estatusValeId: null,
      tipoId: null,
      archivado: false
    });

    const voucherControlFieldsBlank = ref(
      JSON.parse(JSON.stringify(voucherControlFields))
    );
    const fields = ref([
      { value: "controlValeId", text: "ID", sortable: true },
      { value: "fechaEmicion", text: "Fecha emicion" },
      { value: "fechaVigencia", text: "Fecha vigencia" },
      { value: "tipoCombustible", text: "Tipo combustible" },
      { value: "departamentoId", text: "Departamento" },
      { value: "empleadoId", text: "empleado" },
      { value: "proveedorId", text: "Proveedor" },
      { value: "actions", text: "Acciones" },
    ]);

    const resetVoucherControlFields = () => {
      showModal.value = false;
      voucherControlFields.value = JSON.parse(
        JSON.stringify(voucherControlFieldsBlank)
      );
      DateOfIssueState.value = false
      ExpirationDateState.value = false
      FuelTypeState.value = false
      departamentState.value = false
      EmployeeState.value = false
      ProviderState.value = false
      ProductState.value = false
      DetailVoucherState.value = false
      StatusVoucherState.value = false
      TypeVoucherState.value = false
    };

    getVoucherControl((data) => {
      voucherControl.value = data;
      if (voucherControl.value.length > 0) {
        isloading.value = false;
      } else {
        if (voucherControl.value.length <= 0) {
          isloading.value = false;
        }
      }
    });

    const onFiltered = (filteredItems) => {
      currentPage.value = 1;
    };

    const validateDateOfIssue = () => {
      if (!voucherControlFields.value.fechaEmicion) {
        DateOfIssueState.value = false
        return 'Este campo es requerido'
      }
      DateOfIssueState.value = false
      return true
    }

    const validateExpirationDate = () => {
      if (!voucherControlFields.value.fechaVigencia) {
        ExpirationDateState.value = false
        return 'Este campo es requerido'
      }
      ExpirationDateState.value = false
      return true
    }

    const validateFuelType = () => {
      if (!voucherControlFields.value.tipoCombustible) {
        FuelTypeState.value = false;
        return "Este campo es requerido";
      }
      if (!/^[ a-zA-ZñÑáéíóúÁÉÍÓÚ]+$/i.test(voucherControlFields.value.tipoCombustible)) {
        FuelTypeState.value = false;
        return "El nombre del tipo combustible solo puede contener letras";
      }
      if (!voucherControlFields.value.tipoCombustible.trim().length > 0) {
        FuelTypeState.value = false;
        return "Este campo no puede contener espacios";
      }
      FuelTypeState.value = true;
      return true;
    };

    const validateDepartament = () => {
      if (!voucherControlFields.value.departamentoId) {
        departamentState.value = false
        return 'Este campo es requerido'
      }
      departamentState.value = false
      return true
    }

    const validateEmployee = () => {
      if (!voucherControlFields.value.empleadoId) {
        EmployeeState.value = false
        return 'Este campo es requerido'
      }
      EmployeeState.value = false
      return true
    }

    const validateProvider = () => {
      if (!voucherControlFields.value.proveedorId) {
        ProviderState.value = false
        return 'Este campo es requerido'
      }
      ProviderState.value = false
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

    const validateDetailVoucher = () => {
      if (!voucherControlFields.value.detalleValeId) {
        DetailVoucherState.value = false
        return 'Este campo es requerido'
      }
      DetailVoucherState.value = false
      return true
    }

    const validateStatusVoucher = () => {
      if (!voucherControlFields.value.estatusValeId) {
        StatusVoucherState.value = false
        return 'Este campo es requerido'
      }
      StatusVoucherState.value = false
      return true
    }

    const validateTypeVoucher = () => {
      if (!voucherControlFields.value.tipoId) {
        TypeVoucherState.value = false
        return 'Este campo es requerido'
      }
      TypeVoucherState.value = false
      return true
    }

    // pone mis cambios de mis campos vacios de nuevo
    const refreshTable = () => {
      isloading.value = true;
      getVoucherControl((data) => {
        voucherControl.value = data;
        if (voucherControl.value.length > 0) {
          isloading.value = false;
        } else {
          if (voucherControl.value.length <= 0) {
            isloading.value = false;
          }
        }
      });
      return "datos recargados";
    };
    const addWorksStatus = () => {
      createVoucherControl(voucherControlFields.value, (data) => {
        refreshTable();
        swal.fire({
          title: "¡Control de vales registrado correctamente!",
          text:
            "El control de vales se ha registrado al sistema satisfactoriamente.",
          icon: "success",
        });
      });
      showModal.value = false;
      resetVoucherControlFields();
    };
    const RemoveVoucherControl = (VoucherControlId) => {
      isloading.value = true;
      swal
        .fire({
          title: "¿Estas seguro?",
          text: "No podrás revertir esto!",
          icon: "warning",
          showCancelButton: true,
          confirmButtonColor: "#3085d6",
          cancelButtonColor: "#d33",
          confirmButtonText: "Si, Archivar control de vales!",
          cancelButtonText: "Cancelar",
        })
        .then((result) => {
          if (result.isConfirmed) {
            deleteVoucherControl(VoucherControlId, (data) => {
              refreshTable();
            });
            swal.fire({
              title: "¡Control de vales archivado!",
              text:
                "El control de vales ha sido archivado satisfactoriamente .",
              icon: "success",
            });
          } else {
            isloading.value = false;
          }
        });
    };
    return {
      voucherControl,
      voucherControlFields,
      showModal,
      perPage,
      currentPage,
      filter,
      perPageSelect,
      isloading,
      searchValue,
      searchField,
      voucherControlFieldsBlank,
      fields,
      DateOfIssueState,
      ExpirationDateState,
      FuelTypeState,
      departamentState,
      EmployeeState,
      ProviderState,
      ProductState,
      DetailVoucherState,
      StatusVoucherState,
      TypeVoucherState,
      onFiltered,
      addWorksStatus,
      refreshTable,
      RemoveVoucherControl,
      validateDateOfIssue,
      validateExpirationDate,
      validateFuelType,
      validateDepartament,
      validateEmployee,
      validateProvider,
      validateProduct,
      validateDetailVoucher,
      validateStatusVoucher,
      validateTypeVoucher,
      resetVoucherControlFields,
    };
  },
};
</script>

<style></style>
