const app = Vue.createApp({
    data() {
        return {
            productos: [],
            productosFiltrados: [],
            filtro: '',
            modoEdicion: false,
            producto: {
                id: 0,
                nombre: '',
                tipo: '',
                precio: 0,
                cantidad_Stock: 0,
                laboratorio: '',
                requiere_Receta: false,
                fecha_Vencimiento: '',
                activo: true
            }
        };
    },
    methods: {
        async cargarProductos() {
            const res = await fetch('/api/Productos/GetProductos');
            this.productos = await res.json();
            this.productosFiltrados = this.productos;
        },
        filtrarProductos() {
            const texto = this.filtro.toLowerCase();
            this.productosFiltrados = this.productos.filter(p =>
                p.nombre.toLowerCase().includes(texto)
            );
        },
        editarProducto(p) {
            this.producto = { ...p };
            this.modoEdicion = true;
        },
        resetearFormulario() {
            this.producto = {
                id: 0,
                nombre: '',
                tipo: '',
                precio: 0,
                cantidad_Stock: 0,
                laboratorio: '',
                requiere_Receta: false,
                fecha_Vencimiento: '',
                activo: true
            };
            this.modoEdicion = false;
        },
        async guardarProducto() {
            const metodo = this.producto.id ? 'PUT' : 'POST';
            const url = this.producto.id
                ? `/api/Productos/${this.producto.id}`
                : '/api/Productos';

            const res = await fetch(url, {
                method: metodo,
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(this.producto)
            });

            if (!res.ok) {
                alert('Error al guardar');
                return;
            }

            await this.cargarProductos();
            this.resetearFormulario();
        },
        async darDeBaja(p) {
            if (!confirm(`¿Dar de baja a ${p.nombre}?`)) return;

            const baja = { ...p, activo: false };
            const res = await fetch(`/api/Productos/${p.id}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(baja)
            });

            if (!res.ok) {
                alert('Error al dar de baja');
                return;
            }

            await this.cargarProductos();
        }
    },
    mounted() {
        this.cargarProductos();
    }
});

app.mount('#app');
